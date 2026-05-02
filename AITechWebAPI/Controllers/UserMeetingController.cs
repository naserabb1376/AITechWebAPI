using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.UserMeeting;
using AITechWebAPI.Tools;
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
    [Route("UserMeeting")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin,(int)BaseRole.Teacher })]


    public class UserMeetingController : ControllerBase
    {
        IUserMeetingRep _UserMeetingRep;
        IUserRep _UserRep;
        IMeetingRep _MeetingRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public UserMeetingController(IUserMeetingRep UserMeetingRep,IUserRep userRep,IMeetingRep meetingRep,ILogRep logRep,IMapper mapper)
        {
           _UserMeetingRep = UserMeetingRep;
            _UserRep = userRep;
            _MeetingRep = meetingRep;
           _logRep = logRep;
            _mapper = mapper;

        }

        [HttpPost("GetAllUserMeetings_Base")]
        public async Task<ActionResult<ListResultObject<UserMeetingVM>>> GetAllUserMeetings_Base(GetUserMeetingListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserMeetingRep.GetAllUserMeetingsAsync(requestBody.UserId,requestBody.MeetingId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<UserMeetingVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetUserMeetingById_Base")]
        public async Task<ActionResult<RowResultObject<UserMeetingVM>>> GetUserMeetingById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserMeetingRep.GetUserMeetingByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<UserMeetingVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistUserMeeting_Base")]
        public async Task<ActionResult<BitResultObject>> ExistUserMeeting_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserMeetingRep.ExistUserMeetingAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddUserMeetings_Base")]
        public async Task<ActionResult<BitResultObject>> AddUserMeetings_Base(List<AddEditUserMeetingRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var UserMeetings = requestBody.Select(x=> new UserMeeting()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = x.UserId,
                MeetingId = x.MeetingId,
                IsActive = true,
            }).ToList();
            
            var result = await _UserMeetingRep.AddUserMeetingsAsync(UserMeetings);
            if (result.Status)
            {
                foreach (var item in UserMeetings)
                {
                    var user = await _UserRep.GetUserByIdAsync(item.UserId);
                    var meeting = await _MeetingRep.GetMeetingByIdAsync(item.MeetingId);


                    var infoMessage = $@"
{user.Result.FirstName} {user.Result.LastName} عزیز
شما به جلسه {meeting.Result.MeetingTitle} دعوت شده اید
این جلسه
در تاریخ {meeting.Result.MeetingDate.ToShamsiString().Split(' ')[0]}
از ساعت {meeting.Result.MeetingStartTime.TimeSpanToString()}
آغاز می گردد
مرکز آموزش هوش مصنوعی آیتک
";
                    bool sent = await ToolBox.SendSMSMessage(user.Result.Username, infoMessage);

                }

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

        [HttpPut("EditUserMeetings_Base")]
        public async Task<ActionResult<BitResultObject>> EditUserMeetings_Base(List<AddEditUserMeetingRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var UserMeetings = new List<UserMeeting>();

            foreach (var body in requestBody)
            {
                var theRow = await _UserMeetingRep.GetUserMeetingByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var UserMeeting = new UserMeeting
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    UserId = body.UserId,
                    MeetingId = body.MeetingId,
                    IsActive = true,
                };

                UserMeetings.Add(UserMeeting);
            }

            result = await _UserMeetingRep.EditUserMeetingsAsync(UserMeetings);
            if (result.Status)
            {
                foreach (var item in UserMeetings)
                {
                    var user = await _UserRep.GetUserByIdAsync(item.UserId);
                    var meeting = await _MeetingRep.GetMeetingByIdAsync(item.MeetingId);


                    var infoMessage = $@"
{user.Result.FirstName} {user.Result.LastName} عزیز
شما به جلسه {meeting.Result.MeetingTitle} دعوت شده اید
این جلسه
در تاریخ {meeting.Result.MeetingDate.ToShamsiString().Split(' ')[0]}
از ساعت {meeting.Result.MeetingStartTime.TimeSpanToString()}
آغاز می گردد
مرکز آموزش هوش مصنوعی آیتک
";
                    bool sent = await ToolBox.SendSMSMessage(user.Result.Username, infoMessage);

                }

                #region AddLog

                Log log = new Log
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

        [HttpDelete("DeleteUserMeetings_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteUserMeetings_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _UserMeetingRep.RemoveUserMeetingsAsync(ids);
            if (result.Status)
            {
                #region AddLog

                Log log = new Log
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
