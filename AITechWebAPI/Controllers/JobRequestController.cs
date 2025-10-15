using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.JobRequest;
using AITechWebAPI.Models.Public;
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
using System.Net;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("JobRequest")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class JobRequestController : ControllerBase
    {
        IJobRequestRep _JobRequestRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public JobRequestController(IJobRequestRep JobRequestRep,ILogRep logRep,IMapper mapper)
        {
           _JobRequestRep = JobRequestRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllJobRequests_Base")]
        public async Task<ActionResult<ListResultObject<JobRequest>>> GetAllJobRequests_Base(GetJobRequestListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _JobRequestRep.GetAllJobRequestsAsync(requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<JobRequest>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetJobRequestById_Base")]
        public async Task<ActionResult<RowResultObject<JobRequest>>> GetJobRequestById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _JobRequestRep.GetJobRequestByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<JobRequest>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistJobRequest_Base")]
        public async Task<ActionResult<BitResultObject>> ExistJobRequest_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _JobRequestRep.ExistJobRequestAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddJobRequest_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> AddJobRequest_Base(AddEditJobRequestRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            JobRequest JobRequest = new JobRequest()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email,
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,
                CourseTitle = requestBody.CourseTitle,
                RequestedPosition = requestBody.RequestedPosition,
                OtherLangs = requestBody.OtherLangs ?? "",
                Description = requestBody.Description ?? "",
                FatherName = requestBody.FatherName ?? "",
                BirthDate = requestBody.BirthDate.StringToDate(),
                EducationStatus = requestBody.EducationStatus ??"",
                EducationalLevel = requestBody.EducationalLevel ??"",
                LastAcademicLicense = requestBody.LastAcademicLicense ?? "",
                UniversityName = requestBody.UniversityName ?? "",
                NationalCode = requestBody.NationalCode ?? "",
                CheckStatus = requestBody.CheckStatus,
            };
            var result = await _JobRequestRep.AddJobRequestAsync(JobRequest);
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

        [HttpPut("EditJobRequest_Base")]
        public async Task<ActionResult<BitResultObject>> EditJobRequest_Base(AddEditJobRequestRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _JobRequestRep.GetJobRequestByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            JobRequest JobRequest = new JobRequest()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email,
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,
                CourseTitle = requestBody.CourseTitle,
                RequestedPosition = requestBody.RequestedPosition,
                OtherLangs = requestBody.OtherLangs ?? "",
                Description = requestBody.Description ?? "",
                FatherName = requestBody.FatherName ?? "",
                BirthDate = requestBody.BirthDate.StringToDate(),
                EducationStatus = requestBody.EducationStatus ?? "",
                EducationalLevel = requestBody.EducationalLevel ?? "",
                LastAcademicLicense = requestBody.LastAcademicLicense ?? "",
                UniversityName = requestBody.UniversityName ?? "",
                NationalCode = requestBody.NationalCode ?? "",
                CheckStatus = requestBody.CheckStatus,
            };
            result = await _JobRequestRep.EditJobRequestAsync(JobRequest);
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

        [HttpPut("ChangeCheckStatusJobRequest_Base")]
        public async Task<ActionResult<BitResultObject>> ChangeCheckStatusJobRequest_Base(ChangeCheckStatusJobRequestRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _JobRequestRep.GetJobRequestByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

           
            result = await _JobRequestRep.ChangeCheckStatus(requestBody.ID,requestBody.CheckStatus);
            if (result.Status)
            {

                string smsMsg = $@"{theRow.Result.FirstName} {theRow.Result.LastName} عزیز
درخواست همکاری شما تایید شد
لطفا جهت تعیین زمان مصاحبه به سایت مراجعه نمایید
";
                var sentSms = await ToolBox.SendSMSMessage(theRow.Result.PhoneNumber, smsMsg);


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

        [HttpDelete("DeleteJobRequest_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteJobRequest_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _JobRequestRep.RemoveJobRequestAsync(requestBody.ID);
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
