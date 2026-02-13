using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.GroupChatMessage;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Tools;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("GroupChatMessage")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class GroupChatMessageController : ControllerBase
    {
        IGroupChatMessageRep _GroupChatMessageRep;
        IFileUploadRep _fileUploadRep;
        IImageRep _imageRep;
        ILogRep _logRep;
        private readonly IHubContext<GroupChatHub> _hub;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public GroupChatMessageController(IGroupChatMessageRep GroupChatMessageRep,ILogRep logRep,IFileUploadRep fileUploadRep,IImageRep imageRep,IMapper mapper, IHubContext<GroupChatHub> hub,
    IWebHostEnvironment env)
        {
           _GroupChatMessageRep = GroupChatMessageRep;
           _logRep = logRep;
            _fileUploadRep = fileUploadRep;
            _imageRep = imageRep;
            _mapper = mapper;
            _hub = hub;
            _env = env;
        }

        [HttpPost("GetAllGroupChatMessages_Base")]
        public async Task<ActionResult<ListResultObject<GroupChatMessageVM>>> GetAllGroupChatMessages_Base(GetGroupChatMessageListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            long roleId = User.GetCurrentRoleId();
            var result = await _GroupChatMessageRep.GetAllGroupChatMessagesAsync(roleId,requestBody.GroupId,requestBody.SenderUserId,requestBody.ReplyToMessageId,requestBody.WithDeleted,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<GroupChatMessageVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetGroupChatMessageById_Base")]
        public async Task<ActionResult<RowResultObject<GroupChatMessageVM>>> GetGroupChatMessageById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatMessageRep.GetGroupChatMessageByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<GroupChatMessageVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistGroupChatMessage_Base")]
        public async Task<ActionResult<BitResultObject>> ExistGroupChatMessage_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatMessageRep.ExistGroupChatMessageAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddGroupChatMessage_Base")]
        public async Task<ActionResult<BitResultObject>> AddGroupChatMessage_Base(AddEditGroupChatMessageRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            GroupChatMessage GroupChatMessage = new GroupChatMessage()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                SentAt = requestBody.SentAt.ToShamsi(),
                EditedAt = requestBody.EditedAt,
                DeletedAt = requestBody.DeletedAt,
                Text = requestBody.MessageText,
                ReplyToMessageId = requestBody.ReplyToMessageId,
                GroupId = requestBody.GroupId,
                SenderUserId = requestBody.SenderUserId,
                IsActive = requestBody.IsActive,
                IsEdited = requestBody.IsEdited,
                IsDeleted = requestBody.IsDeleted,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _GroupChatMessageRep.AddGroupChatMessageAsync(GroupChatMessage);
            if (result.Status)
            {
                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                };
                await _logRep.AddLogAsync(log);

                #endregion


                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("EditGroupChatMessage_Base")]
        public async Task<ActionResult<BitResultObject>> EditGroupChatMessage_Base(AddEditGroupChatMessageRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _GroupChatMessageRep.GetGroupChatMessageByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            GroupChatMessage GroupChatMessage = new GroupChatMessage()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                SentAt = theRow.Result.SentAt,
                EditedAt = requestBody.EditedAt ?? DateTime.Now.ToShamsi(),
                DeletedAt = requestBody.DeletedAt,
                Text = requestBody.MessageText,
                ReplyToMessageId = requestBody.ReplyToMessageId,
                GroupId = requestBody.GroupId,
                SenderUserId = requestBody.SenderUserId,
                IsActive = requestBody.IsActive,
                IsEdited = requestBody.IsEdited,
                IsDeleted = requestBody.IsDeleted,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _GroupChatMessageRep.EditGroupChatMessageAsync(GroupChatMessage);
            if (result.Status)
            {

                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                };
                await _logRep.AddLogAsync(log);

                #endregion

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteGroupChatMessage_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteGroupChatMessage_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatMessageRep.RemoveGroupChatMessageAsync(requestBody.ID);
            if (result.Status)
            {

                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                };
                await _logRep.AddLogAsync(log);

                #endregion

                return Ok(result);
            }
            return BadRequest(result);
        }


        // Messenger Actions

        [HttpPost("GetGroupMessages")]
        public async Task<ActionResult<List<GroupChatMessageDto>>> GetGroupMessages(GetGroupMessagesRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            long roleId = User.GetCurrentRoleId();
            var senderUserId = User.GetCurrentUserId();
            var result = await _GroupChatMessageRep.GetMessagesAsync(roleId,requestBody.GroupId,senderUserId, requestBody.PageIndex, requestBody.PageSize);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("SendMessageGroupChat")]
        public async Task<ActionResult<GroupChatMessageDto>> SendMessageGroupChat(SendMessageRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var senderUserId = User.GetCurrentUserId();
            long roleId = User.GetCurrentRoleId();
            var result = await _GroupChatMessageRep.SendMessageAsync(roleId,requestBody.GroupId,senderUserId, new SendGroupMessageRequest() { ReplyToMessageId=requestBody.ReplyToMessageId,Text = requestBody.MessageText});
            if (result != null)
            {

                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                };
                await _logRep.AddLogAsync(log);

                #endregion

                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPut("EditMessageGroupChat")]
        public async Task<ActionResult<GroupChatMessageDto>> EditMessageGroupChat(EditMessageRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var senderUserId = User.GetCurrentUserId();
            long roleId = User.GetCurrentRoleId();
            var result = await _GroupChatMessageRep.EditMessageAsync(roleId,requestBody.MessageId,requestBody.GroupId, senderUserId, new EditGroupMessageRequest() { Text = requestBody.MessageText });
            if (result != null)
            {

                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                };
                await _logRep.AddLogAsync(log);

                #endregion

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("SoftDeleteMessageGroupChat")]
        public async Task<ActionResult> SoftDeleteMessageGroupChat(SoftDeleteMessageRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {

                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

                };
                await _logRep.AddLogAsync(log);

                #endregion


                return BadRequest(requestBody);
            }
            var senderUserId = User.GetCurrentUserId();
            long roleId = User.GetCurrentRoleId();
            await _GroupChatMessageRep.SoftDeleteMessageAsync(roleId, requestBody.GroupId,requestBody.MessageId,senderUserId);
            return NoContent();
        }

        [HttpPost("send-with-attachment")]
        public async Task<ActionResult<SendWithAttachmentResponse>> SendWithAttachment(
      [FromForm] IFormFile file,
      [FromForm] UploadChatAttachmentRequestBody request)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("فایلی انتخاب نشده است.");

            var userId = User.GetCurrentUserId();
            long roleId = User.GetCurrentRoleId();

            // 1) اول پیام رو بساز تا messageId داشته باشیم
            var created = await _GroupChatMessageRep.SendMessageAsync(roleId,request.GroupId, userId, new SendGroupMessageRequest
            {
                Text = request.MessageText ?? "📎 فایل ارسال شد",
                ReplyToMessageId = null
            });

            // 2) ذخیره فایل با EntityName=GroupChatMessage و RowId=MessageId
            var uploadResult = await SaveFileForChatMessageAsync(
                file: file,
                entityName: "GroupChatMessage",
                rowId: created.Id,
                fileType: request.FileType,
                isPublic: request.IsPublic,
                userId: userId
            );


            #region AddLog

            Log log = new Log()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                LogTime = DateTime.Now.ToShamsi(),
                ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

            };
            await _logRep.AddLogAsync(log);

            #endregion


            // 3) فقط خروجی لازم رو برگردون (Attach را از طریق Hub انجام می‌دهیم)
            return Ok(new SendWithAttachmentResponse
            {
                GroupId = request.GroupId,
                MessageId = created.Id,
                Url = uploadResult.Url,
                FileName = uploadResult.FileName,
                Size = uploadResult.Size,
                FileType = uploadResult.FileType
            });
        }


        private async Task<(string FileName, string Url, long ResultId, long Size, string FileType)> SaveFileForChatMessageAsync(
    IFormFile file,
    string entityName,
    long rowId,
    string fileType,
    bool isPublic,
    long userId
 )
        {
            fileType = fileType.ToLower();
            entityName = entityName.ToLower();

            if (fileType != "images" && fileType != "files")
                throw new ArgumentException("نوع فایل باید images یا files باشد");

            string fileName = "";
            string fullPath = "";
            long rowNumber = 0;
            long resultId = 0;
            string downloadUrl = "";

            // مسیر ذخیره
            string savePath = isPublic
                ? Path.Combine(_env.ContentRootPath, "FileCenter", entityName, fileType, "Public")
                : Path.Combine(_env.ContentRootPath, "FileCenter", entityName, fileType, userId.ToString());

            Directory.CreateDirectory(savePath);

            if (fileType == "images")
            {
                rowNumber = await _imageRep.GetNewRowNumber();
                fileName = $"{entityName}_{rowNumber}_{userId}{Path.GetExtension(file.FileName)}";
                fullPath = Path.Combine(savePath, fileName);

                var theImage = new Image
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    Description = isPublic ? "Public" : "Private",
                    FileName = fileName,
                    FilePath = fullPath,
                    EntityType = entityName,
                    ForeignKeyId = rowId,
                    IsActive = true,
                    Note = null,
                    Tag = null,
                    CreatorId = userId,
                };

                // اگر برای پیام چت می‌خوای چند فایل داشته باشی، این خط رو حذف کن
                // (الان RemoveOldImages باعث میشه هر پیام فقط یک عکس داشته باشه)
                // var removeoldResult = await _imageRep.RemoveOldImagesAsync(rowId, entityName);
                // if (!removeoldResult.Status) throw new ArgumentException(removeoldResult.ErrorMessage);

                var saveResult = await _imageRep.AddImagesAsync(new List<Image> { theImage });
                if (!saveResult.Status) throw new ArgumentException(saveResult.ErrorMessage);

                resultId = saveResult.ID;
                downloadUrl = $"/filecenter/downloadfile?fileType={fileType}&rowId={resultId}&entityName={entityName}";
                theImage.GetUrl = downloadUrl;
                await _imageRep.EditImagesAsync(new List<Image> { theImage });
            }
            else // files
            {
                rowNumber = await _fileUploadRep.GetNewRowNumber();
                fileName = $"{entityName}_{rowNumber}_{userId}{Path.GetExtension(file.FileName)}";
                fullPath = Path.Combine(savePath, fileName);

                var theFile = new FileUpload
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    FileName = fileName,
                    FilePath = fullPath,
                    EntityType = entityName,
                    ForeignKeyId = rowId,
                    ContentType = fullPath.GetContentType(),
                    Description = isPublic ? "Public" : "Private",
                    IsActive = true,
                    Note = null,
                    Tag = null,
                    CreatorId = userId,
                };

                // اگر برای پیام چت می‌خوای چند فایل داشته باشی، این خط رو حذف کن
                // var removeoldResult = await _fileUploadRep.RemoveOldFilesAsync(rowId, entityName);
                // if (!removeoldResult.Status) throw new ArgumentException(removeoldResult.ErrorMessage);

                var saveResult = await _fileUploadRep.AddFileUploadAsync(theFile);
                if (!saveResult.Status) throw new ArgumentException(saveResult.ErrorMessage);

                resultId = saveResult.ID;
                downloadUrl = $"/filecenter/downloadfile?fileType={fileType}&rowId={resultId}&entityName={entityName}";
                theFile.GetUrl = downloadUrl;
                await _fileUploadRep.EditFileUploadAsync(theFile);
            }

            // ذخیره فایل فیزیکی
            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);


            #region AddLog

            Log log = new Log()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                LogTime = DateTime.Now.ToShamsi(),
                ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

            };
            await _logRep.AddLogAsync(log);

            #endregion


            return (fileName, downloadUrl, resultId, file.Length, fileType);
        }

    }
}
