using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models.FileCenter;
using AITechWebAPI.Models.FileUpload;
using AITechWebAPI.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

[Route("FileCenter")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class FileCenterController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogRep _logRep;
    private readonly IFileUploadRep _fileUploadRep;
    private readonly IImageRep _imageRep;

    public FileCenterController(IWebHostEnvironment env, ILogRep logRep, IFileUploadRep fileUploadRep, IImageRep imageRep)
    {
        _env = env;
        _logRep = logRep;
        _fileUploadRep = fileUploadRep;
        _imageRep = imageRep;
    }

    [HttpPost("uploadfile")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] UploadFileRequestBody requestBody)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("فایلی انتخاب نشده است.");

            var fileType = requestBody.FileType.ToLower();
            var entityName = requestBody.EntityName.ToLower();
            bool isJobRequest = entityName.ToLower() == "jobrequest" || entityName.ToLower() == "preregistration";

            string fileName = "", fullPath=""; long RowNumber =0;    
            var userId = !isJobRequest ? User?.FindFirst("userId")?.Value : "0";
            if ((string.IsNullOrEmpty(userId) || userId == "0") && !isJobRequest) return Unauthorized();

            string savePath = requestBody.IsPublic
                ? Path.Combine(_env.ContentRootPath, "FileCenter", entityName, fileType, "Public")
                : Path.Combine(_env.ContentRootPath, "FileCenter", entityName, fileType, userId);

            Directory.CreateDirectory(savePath);
            long resultId = 0;
            string downloadUrl = "";

            if (fileType == "images")
            {
                RowNumber = await _imageRep.GetNewRowNumber();
                fileName = $"{entityName}_{RowNumber}_{userId}{Path.GetExtension(file.FileName)}";
                fullPath = Path.Combine(savePath, fileName);
                Image theImage = new()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    Description = requestBody.IsPublic ? "Public" : "Private",
                    FileName = fileName,
                    FilePath = fullPath,
                    EntityType = entityName,
                    ForeignKeyId = requestBody.RowId <= 0 ? RowNumber : requestBody.RowId,
                    IsActive = true,
                    Note = requestBody.Note,
                    Tag = requestBody.Tag,
                    CreatorId = long.Parse(userId),
                };
                var removeoldResult = await _imageRep.RemoveOldImagesAsync(requestBody.RowId,entityName);
                if (!removeoldResult.Status) return BadRequest(removeoldResult);

                var saveResult = await _imageRep.AddImagesAsync(new List<Image> { theImage });
                if (!saveResult.Status) return BadRequest(saveResult);
                resultId = saveResult.ID;

                downloadUrl = $"/filecenter/downloadfile?fileType={fileType}&rowId={resultId}&entityName={entityName}";
                theImage.GetUrl = downloadUrl;
                await _imageRep.EditImagesAsync(new List<Image>() { theImage });

            }
            else if (fileType == "files")
            {
                RowNumber = await _fileUploadRep.GetNewRowNumber();
                fileName = $"{entityName}_{RowNumber}_{userId}{Path.GetExtension(file.FileName)}";
                fullPath = Path.Combine(savePath, fileName);

                FileUpload theFile = new()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    FileName = fileName,
                    FilePath = fullPath,
                    EntityType= entityName,
                    ForeignKeyId = requestBody.RowId <= 0 ? RowNumber : requestBody.RowId,
                    ContentType = fullPath.GetContentType(),
                    Description = requestBody.IsPublic ? "Public" : "Private",
                    IsActive = true,
                    Note = requestBody.Note,
                    Tag = requestBody.Tag,
                    CreatorId = long.Parse(userId),
                };
                var removeoldResult = await _fileUploadRep.RemoveOldFilesAsync(requestBody.RowId, entityName);
                if (!removeoldResult.Status) return BadRequest(removeoldResult);

                var saveResult = await _fileUploadRep.AddFileUploadAsync(theFile);
                if (!saveResult.Status) return BadRequest(saveResult);
                resultId = saveResult.ID;

                downloadUrl = $"/filecenter/downloadfile?fileType={fileType}&rowId={resultId}&entityName={entityName}";
                theFile.GetUrl = downloadUrl;
                await _fileUploadRep.EditFileUploadAsync(theFile);


            }
            else return BadRequest("Invalid File Category!");

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            Log log = new()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                LogTime = DateTime.Now.ToShamsi(),
                ActionName = $"UploadFile:{{Entity={entityName},Type={fileType},Row={requestBody.RowId},Path={fullPath},Id={resultId}}}",
            };
            await _logRep.AddLogAsync(log);

            return Ok(new
            {
                success = true,
                fileName,
                resultId,
                url = downloadUrl,
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"{ex.Message} - {ex.InnerException?.Message}");
        }
    }

    [HttpGet("downloadfile")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadFile([FromQuery] string fileType, [FromQuery] long rowId = 0, [FromQuery] long foreignkeyId = 0, [FromQuery] string entityName = "")
    {
        try
        {
            string filePath = string.Empty;
            long userId = 0;
            long roleId = 0;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = long.Parse(User.FindFirst("userId")?.Value ?? "0");
                roleId = long.Parse(User.FindFirst("Role")?.Value ?? "0");
            }

            if (fileType.ToLower() == "images")
            {
                var theImage = await _imageRep.GetImageForShowAsync(rowId, foreignkeyId, entityName, userId, roleId);
                if (theImage != null) filePath = theImage.Result.FilePath;
            }
            else if (fileType.ToLower() == "files")
            {
                var theFile = await _fileUploadRep.GetFileForDownloadAsync(rowId, foreignkeyId, entityName, userId, roleId);
                if (theFile != null) filePath = theFile.Result.FilePath;
            }
            else return BadRequest("Invalid File Category!");

            if (!System.IO.File.Exists(filePath)) return NotFound();

            Log log = new()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                LogTime = DateTime.Now.ToShamsi(),
                ActionName = $"DownloadFile:{{Entity={entityName},Type={fileType},Row={rowId},FK={foreignkeyId},Path={filePath}}}",
            };
            await _logRep.AddLogAsync(log);

            var contentType = filePath.GetContentType();
            var fileName = Path.GetFileName(filePath);
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"{ex.Message} - {ex.InnerException?.Message}");
        }
    }

    [HttpPost("GetDownloadLinks_Base")]
    public async Task<ActionResult<ListResultObject<string>>> GetDownloadLinks_Base(GetFileCenterDownloadListRequestBody requestBody)
    {
        requestBody.entityType= requestBody.entityType.ToLower();
        requestBody.fileType= requestBody.fileType.ToLower();

        var result = new ListResultObject<string>();
        dynamic resultrecords;

        if (!ModelState.IsValid)
        {
            return BadRequest(requestBody);
        }
        switch (requestBody.fileType)
        {
            default:
            case "files":
                {
                    resultrecords = await _fileUploadRep.GetAllFileUploadsAsync(requestBody.entityType, requestBody.ForeignKeyId, 0, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
                }
                break;
            case "images":
                {
                    resultrecords = await _imageRep.GetAllImagesAsync(requestBody.entityType, requestBody.ForeignKeyId, 0, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
                }
                break;
        }

        result.ErrorMessage = resultrecords.ErrorMessage;
        result.Status = resultrecords.Status;
        result.PageCount = resultrecords.PageCount;
        result.TotalCount = resultrecords.TotalCount;

        if (requestBody.fileType == "images")
        {
            result.Results = ((List<Image>)resultrecords.Results)
  .Select(x => x.FilePath).ToList();
        }
        if (requestBody.fileType == "files")
        {
            result.Results = ((List<FileUpload>)resultrecords.Results)
  .Select(x => x.FilePath).ToList();
        }

        if (result.Status)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }


   
}