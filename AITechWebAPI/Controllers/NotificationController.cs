using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.News;
using AITechWebAPI.Models.Notification;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("Notification")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class NotificationController : ControllerBase
    {
        INotificationRep _NotificationRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public NotificationController(INotificationRep NotificationRep,ILogRep logRep,IMapper mapper)
        {
           _NotificationRep = NotificationRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllNotifications_Base")]
        public async Task<ActionResult<ListResultObject<NotificationVM>>> GetAllNotifications_Base(GetNewsListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NotificationRep.GetAllNotificationsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<NotificationVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetNotificationById_Base")]
        public async Task<ActionResult<RowResultObject<NotificationVM>>> GetNotificationById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NotificationRep.GetNotificationByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<NotificationVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistNotification_Base")]
        public async Task<ActionResult<BitResultObject>> ExistNotification_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NotificationRep.ExistNotificationAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddNotification_Base")]
        public async Task<ActionResult<BitResultObject>> AddNotification_Base(AddEditNotificationRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Notification Notification = new Notification()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserID,
                Message = requestBody.Message,
                IsRead = requestBody.NotificationSeenStatus,
                OtherLangs = requestBody.OtherLangs ?? "",
                //IsRead = requestBody.SentDate ?? DateTime.Now.ToShamsi(),
                // Description = requestBody.Description,
            };
            var result = await _NotificationRep.AddNotificationAsync(Notification);
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

        [HttpPut("EditNotification_Base")]
        public async Task<ActionResult<BitResultObject>> EditNotification_Base(AddEditNotificationRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _NotificationRep.GetNotificationByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Notification Notification = new Notification()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserID,
                Message = requestBody.Message,
                IsRead = requestBody.NotificationSeenStatus,
                OtherLangs = requestBody.OtherLangs ?? "",
                //SentDate = requestBody.SentDate ?? DateTime.Now.ToShamsi(),
                //Description = requestBody.Description,
            };
            result = await _NotificationRep.EditNotificationAsync(Notification);
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

        [HttpDelete("DeleteNotification_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteNotification_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NotificationRep.RemoveNotificationAsync(requestBody.ID);
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
    }
}
