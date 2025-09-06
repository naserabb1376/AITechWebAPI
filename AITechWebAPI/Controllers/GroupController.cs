using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Group;
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
    [Route("Group")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]

    public class GroupController : ControllerBase
    {
        private IGroupRep _GroupRep;
        private readonly IMapper _mapper;
        private ILogRep _logRep;

        public GroupController(IGroupRep GroupRep, ILogRep logRep,IMapper mapper)
        {
            _GroupRep = GroupRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllGroups_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<ListResultObject<GroupVM>>> GetAllGroups_Base(GetGroupListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupRep.GetAllGroupsAsync(requestBody.StudentId,requestBody.CourseId,requestBody.TeacherId, requestBody.GroupStatus, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<GroupVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetGroupById_Base")]
        public async Task<ActionResult<RowResultObject<GroupVM>>> GetGroupById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupRep.GetGroupByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<GroupVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

      

        [HttpPost("ExistGroup_Base")]
        public async Task<ActionResult<BitResultObject>> ExistGroup_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupRep.ExistGroupAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddGroup_Base")]
        public async Task<ActionResult<BitResultObject>> AddGroup_Base(AddEditGroupRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Group Group = new Group()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                CourseId = requestBody.CourseId,
                TeacherId = requestBody.TeacherId,
                DayOfWeek = requestBody.DayOfWeek,
                StartDate = requestBody.StartDate.StringToDate().Value,
                EndDate = requestBody.EndDate.StringToDate().Value,
                EndTime = requestBody.EndTime?.StringToTimeSpan(),
                StartTime = requestBody.StartTime.StringToTimeSpan(),
                Fee = requestBody.GroupFee,
                Name = requestBody.Name,
                Note = requestBody.Note ?? "",
                OtherLangs = requestBody.OtherLangs ?? "",
                Status = (GroupStatus)Enum.Parse(typeof(GroupStatus), requestBody.GroupStatus),
            };
            var result = await _GroupRep.AddGroupAsync(Group);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("EditGroup_Base")]
        public async Task<ActionResult<BitResultObject>> EditGroup_Base(AddEditGroupRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _GroupRep.GetGroupByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Group Group = new Group()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                CourseId = requestBody.CourseId,
                TeacherId = requestBody.TeacherId,
                DayOfWeek = requestBody.DayOfWeek,
                StartDate = requestBody.StartDate.StringToDate().Value,
                EndDate = requestBody.EndDate.StringToDate().Value,
                EndTime = requestBody.EndTime?.StringToTimeSpan(),
                StartTime = requestBody.StartTime.StringToTimeSpan(),
                Fee = requestBody.GroupFee,
                Name = requestBody.Name,
                Note = requestBody.Note ??"",
                OtherLangs = requestBody.OtherLangs ?? "",
                Status = (GroupStatus)Enum.Parse(typeof(GroupStatus), requestBody.GroupStatus),
            };
            result = await _GroupRep.EditGroupAsync(Group);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteGroup_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteGroup_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GroupRep.RemoveGroupAsync(requestBody.ID);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}