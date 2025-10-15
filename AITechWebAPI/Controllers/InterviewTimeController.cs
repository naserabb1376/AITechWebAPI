using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.InterviewTime;
using AITechWebAPI.Models.InterviewTime;
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
using AITechWebAPI.Tools;

namespace AITechWebAPI.Controllers
{
    [Route("InterviewTime")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class InterviewTimeController : ControllerBase
    {
        IInterviewTimeRep _InterviewTimeRep;
        IJobRequestRep _jobRequestRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public InterviewTimeController(IInterviewTimeRep InterviewTimeRep,IJobRequestRep jobRequestRep,ILogRep logRep,IMapper mapper)
        {
           _InterviewTimeRep = InterviewTimeRep;
            _jobRequestRep = jobRequestRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllInterviewTimes_Base")]
        public async Task<ActionResult<ListResultObject<InterviewTimeVM>>> GetAllInterviewTimes_Base(GetInterviewTimeListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _InterviewTimeRep.GetAllInterviewTimesAsync(requestBody.JobRequestId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<InterviewTimeVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetInterviewTimeById_Base")]
        public async Task<ActionResult<RowResultObject<InterviewTimeVM>>> GetInterviewTimeById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _InterviewTimeRep.GetInterviewTimeByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<InterviewTimeVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistInterviewTime_Base")]
        public async Task<ActionResult<BitResultObject>> ExistInterviewTime_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _InterviewTimeRep.ExistInterviewTimeAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetInterviewTimesInDay_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<ListResultObject<InterviewSlot>>> GetInterviewTimesInDay_Base(GetInterviewTimesInDayRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _InterviewTimeRep.GetInterviewTimesInDayAsync(requestBody.InterviewDate, requestBody.InterviewStartTime, requestBody.InterviewEndTime, requestBody.InterviewMinutes, requestBody.PageIndex, requestBody.PageSize);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddInterviewTime_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> AddInterviewTime_Base(AddEditInterviewTimeRequestBody requestBody)
        {
            BitResultObject result = new BitResultObject();
            JobRequest jobRequest = new JobRequest();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var theInterview = await _InterviewTimeRep.GetAllInterviewTimesAsync(requestBody.JobRequestId, 1, 0);
            if (requestBody.JobRequestId > 0)
            {
                var theJobReq = await _jobRequestRep.GetJobRequestByIdAsync(requestBody.JobRequestId);
                jobRequest = theJobReq.Result;
            }
            else if (!string.IsNullOrEmpty(requestBody.PhoneNumber))
            {
                var theJobReq = await _jobRequestRep.GetAllJobRequestsAsync(1,0,requestBody.PhoneNumber);
                jobRequest = theJobReq.Results.LastOrDefault();
            }

            if (jobRequest.ID <= 0)
            {
                result.Status = false;
                result.ErrorMessage = "این درخواست نامعتبر است";
                return BadRequest(result);
            }

            InterviewTime InterviewTime = new InterviewTime()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                OtherLangs = requestBody.OtherLangs,
                InterviewDate = requestBody.InterviewDate,
                JobRequestId = requestBody.JobRequestId,
                InterviewStartTime = requestBody.InterviewStartTime,
                InterviewEndTime = requestBody.InterviewEndTime,
            };

            if (theInterview.Results.Count > 0)
            {
                InterviewTime.ID = theInterview.Results.FirstOrDefault().ID;
            }

            result = await _InterviewTimeRep.EditInterviewTimeAsync(InterviewTime);
            if (result.Status)
            {
                string smsMsg = $@"{jobRequest.FirstName} {jobRequest.LastName} عزیز
وقت مصاحبه شما برای ساعت {InterviewTime.InterviewStartTime} روز {InterviewTime.InterviewDate}
تنظیم شد
";
                var sentSms = await ToolBox.SendSMSMessage(jobRequest.PhoneNumber,smsMsg);

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

        [HttpPut("EditInterviewTime_Base")]
        public async Task<ActionResult<BitResultObject>> EditInterviewTime_Base(AddEditInterviewTimeRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _InterviewTimeRep.GetInterviewTimeByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            InterviewTime InterviewTime = new InterviewTime()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                OtherLangs = requestBody.OtherLangs,
                InterviewDate = requestBody.InterviewDate,
                JobRequestId = requestBody.JobRequestId,
                InterviewStartTime = requestBody.InterviewStartTime,
                InterviewEndTime = requestBody.InterviewEndTime,
            };
            result = await _InterviewTimeRep.EditInterviewTimeAsync(InterviewTime);
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

        [HttpDelete("DeleteInterviewTime_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteInterviewTime_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _InterviewTimeRep.RemoveInterviewTimeAsync(requestBody.ID);
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
