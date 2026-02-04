using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Meeting;
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
    [Route("Meeting")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]

    public class MeetingController : ControllerBase
    {
        private IMeetingRep _MeetingRep;
        private readonly IMapper _mapper;
        private ILogRep _logRep;

        public MeetingController(IMeetingRep MeetingRep, ILogRep logRep,IMapper mapper)
        {
            _MeetingRep = MeetingRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllMeetings_Base")]
        public async Task<ActionResult<ListResultObject<MeetingVM>>> GetAllMeetings_Base(GetMeetingListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MeetingRep.GetAllMeetingsAsync(requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<MeetingVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetMeetingById_Base")]
        public async Task<ActionResult<RowResultObject<MeetingVM>>> GetMeetingById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MeetingRep.GetMeetingByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<MeetingVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

      

        [HttpPost("ExistMeeting_Base")]
        public async Task<ActionResult<BitResultObject>> ExistMeeting_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MeetingRep.ExistMeetingAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddMeeting_Base")]
        public async Task<ActionResult<BitResultObject>> AddMeeting_Base(AddEditMeetingRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Meeting Meeting = new Meeting()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                MeetingAttendees = requestBody.MeetingAttendees,
                MeetingStatus = requestBody.MeetingStatus,
                MeetingDate = requestBody.MeetingDate.StringToDate().Value,
                MeetingStartTime = requestBody.MeetingStartTime.StringToTimeSpan(),
                MeetingTitle = requestBody.MeetingTitle ?? "",
                MeetingOrganizer = requestBody.MeetingOrganizer ?? "",
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _MeetingRep.AddMeetingAsync(Meeting);
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

        [HttpPut("EditMeeting_Base")]
        public async Task<ActionResult<BitResultObject>> EditMeeting_Base(AddEditMeetingRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _MeetingRep.GetMeetingByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Meeting Meeting = new Meeting()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                MeetingAttendees = requestBody.MeetingAttendees,
                MeetingStatus = requestBody.MeetingStatus,
                MeetingDate = requestBody.MeetingDate.StringToDate().Value,
                MeetingStartTime = requestBody.MeetingStartTime.StringToTimeSpan(),
                MeetingTitle = requestBody.MeetingTitle ?? "",
                MeetingOrganizer = requestBody.MeetingOrganizer ?? "",
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _MeetingRep.EditMeetingAsync(Meeting);
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

        [HttpDelete("DeleteMeeting_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteMeeting_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MeetingRep.RemoveMeetingAsync(requestBody.ID);
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