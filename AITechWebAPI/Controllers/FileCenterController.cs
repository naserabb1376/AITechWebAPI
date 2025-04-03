using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.FileProviders;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.Tools;

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
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] bool isPublic, [FromQuery] string entityName, [FromQuery] string fileType, [FromQuery] long rowId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("فایلی انتخاب نشده است.");

        var fileName = Path.GetFileName(file.FileName);
        var userId = User?.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        string savePath = isPublic
            ? Path.Combine(_env.ContentRootPath, "FileCenter", entityName, fileType, "Public")
            : Path.Combine(_env.ContentRootPath, "FileCenter", entityName, fileType, userId);

        Directory.CreateDirectory(savePath);
        var fullPath = Path.Combine(savePath, fileName);
        long resultId = 0;

        if (fileType.ToLower() == "images")
        {
            Image theImage = new()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Description = isPublic ? "Public" : "Private",
                FileName = fileName,
                FilePath = fullPath,
                EntityType = entityName,
                ForeignKeyId = rowId,
                CreatorId = long.Parse(userId),
            };
            var saveResult = await _imageRep.AddImagesAsync(new List<Image> { theImage });
            if (!saveResult.Status) return BadRequest(saveResult);
            resultId = saveResult.ID;
        }
        else if (fileType.ToLower() == "files")
        {
            FileUpload theFile = new()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                FileName = fileName,
                FilePath = fullPath,
                AssignmentId = rowId,
                ContentType = GetContentType(fullPath),
                Description = isPublic ? "Public" : "Private",
                CreatorId = long.Parse(userId),
            };
            var saveResult = await _fileUploadRep.AddFileUploadAsync(theFile);
            if (!saveResult.Status) return BadRequest(saveResult);
            resultId = saveResult.ID;
        }
        else return BadRequest("Invalid File Category!");

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        Log log = new()
        {
            CreateDate = DateTime.Now.ToShamsi(),
            UpdateDate = DateTime.Now.ToShamsi(),
            LogTime = DateTime.Now.ToShamsi(),
            ActionName = $"UploadFile:{{Entity={entityName},Type={fileType},Row={rowId},Path={fullPath},Id={resultId}}}",
        };
        await _logRep.AddLogAsync(log);

        return Ok(new
        {
            success = true,
            fileName,
            resultId,
            url = $"/filecenter/downloadfile?fileType={fileType}&rowId={resultId}&entityName={entityName}"
        });
    }

    [HttpGet("downloadfile")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadFile([FromQuery] string fileType, [FromQuery] long rowId = 0, [FromQuery] long foreignkeyId = 0, [FromQuery] string entityName = "")
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
            var theFile = await _fileUploadRep.GetFileForDownloadAsync(rowId, foreignkeyId, userId, roleId);
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

        var contentType = GetContentType(filePath);
        var fileName = Path.GetFileName(filePath);
        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(bytes, contentType, fileName);
    }

    private string GetContentType(string path)
    {
        var types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".pdf"] = "application/pdf",
            [".mp4"] = "video/mp4",
            [".txt"] = "text/plain",
            [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        var ext = Path.GetExtension(path);
        return types.TryGetValue(ext, out var contentType) ? contentType : "application/octet-stream";
    }
}
