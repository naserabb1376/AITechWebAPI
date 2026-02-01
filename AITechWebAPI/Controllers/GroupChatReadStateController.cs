using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.GroupChatReadState;
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
    [Route("GroupChatReadState")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class GroupChatReadStateController : ControllerBase
    {
        IGroupChatReadStateRep _GroupChatReadStateRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;

        public GroupChatReadStateController(IGroupChatReadStateRep GroupChatReadStateRep,ILogRep logRep,IMapper mapper)
        {
           _GroupChatReadStateRep = GroupChatReadStateRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllGroupChatReadStates_Base")]
        public async Task<ActionResult<ListResultObject<GroupChatReadStateVM>>> GetAllGroupChatReadStates_Base(GetGroupChatReadStateListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatReadStateRep.GetAllGroupChatReadStatesAsync(requestBody.GroupId,requestBody.UserId,requestBody.LastReadMessageId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<GroupChatReadStateVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetGroupChatReadStateById_Base")]
        public async Task<ActionResult<RowResultObject<GroupChatReadStateVM>>> GetGroupChatReadStateById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatReadStateRep.GetGroupChatReadStateByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<GroupChatReadStateVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistGroupChatReadState_Base")]
        public async Task<ActionResult<BitResultObject>> ExistGroupChatReadState_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatReadStateRep.ExistGroupChatReadStateAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddGroupChatReadState_Base")]
        public async Task<ActionResult<BitResultObject>> AddGroupChatReadState_Base(AddEditGroupChatReadStateRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            GroupChatReadState GroupChatReadState = new GroupChatReadState()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                LastReadMessageId = requestBody.LastReadMessageId,
                GroupId = requestBody.GroupId,
                UserId = requestBody.UserId,
                IsActive = requestBody.IsActive,
                LastReadAt = requestBody.LastReadAt,               
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _GroupChatReadStateRep.AddGroupChatReadStateAsync(GroupChatReadState);
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

        [HttpPut("EditGroupChatReadState_Base")]
        public async Task<ActionResult<BitResultObject>> EditGroupChatReadState_Base(AddEditGroupChatReadStateRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _GroupChatReadStateRep.GetGroupChatReadStateByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            GroupChatReadState GroupChatReadState = new GroupChatReadState()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                LastReadMessageId = requestBody.LastReadMessageId,
                GroupId = requestBody.GroupId,
                UserId = requestBody.UserId,
                IsActive = requestBody.IsActive,
                LastReadAt = requestBody.LastReadAt,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _GroupChatReadStateRep.EditGroupChatReadStateAsync(GroupChatReadState);
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

        [HttpDelete("DeleteGroupChatReadState_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteGroupChatReadState_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupChatReadStateRep.RemoveGroupChatReadStateAsync(requestBody.ID);
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


        // Seen Actions

        [HttpPost("GetMyReadState")]
        public async Task<ActionResult<GroupChatReadStateDto>> GetMyReadState(GetReadStateRequestBody requestBody)
        {
            var userId = User.GetCurrentUserId();

            var result = await _GroupChatReadStateRep.GetReadStateAsync(requestBody.GroupId, userId);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        [HttpPost("GetMembersReadStates")]
        public async Task<ActionResult<List<GroupMemberReadStateDto>>> GetMembersReadStates(GetReadStateRequestBody requestBody)
        {
            var userId = User.GetCurrentUserId();

            var result = await _GroupChatReadStateRep.GetGroupReadStatesAsync(requestBody.GroupId, userId);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        [HttpPost("MarkAsSeen")]
        public async Task<IActionResult> MarkAsSeen(SeenRequest requestBody)
        {
            if (requestBody.LastReadMessageId <= 0)
                throw new ArgumentException("آخرین شناسه پیام معتبر نیست");

            var userId = User.GetCurrentUserId();

            await _GroupChatReadStateRep.MarkAsSeenAsync(requestBody.GroupId, userId, requestBody.LastReadMessageId);
            return Ok(new { success = true });
        }
    }
}
