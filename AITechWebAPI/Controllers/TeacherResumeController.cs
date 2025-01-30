using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.TeacherResume;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AITechWebAPI.Controllers
{
    [Route("TeacherResume")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class TeacherResumeController : ControllerBase
    {
        ITeacherResumeRep _TeacherResumeRep;
        ILogRep _logRep;

        public TeacherResumeController(ITeacherResumeRep TeacherResumeRep,ILogRep logRep)
        {
           _TeacherResumeRep = TeacherResumeRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllTeacherResumes_Base")]
        public async Task<ActionResult<ListResultObject<TeacherResume>>> GetAllTeacherResumes_Base(GetTeacherResumeListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TeacherResumeRep.GetAllTeacherResumesAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetTeacherResumeById_Base")]
        public async Task<ActionResult<RowResultObject<TeacherResume>>> GetTeacherResumeById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TeacherResumeRep.GetTeacherResumeByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistTeacherResume_Base")]
        public async Task<ActionResult<BitResultObject>> ExistTeacherResume_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TeacherResumeRep.ExistTeacherResumeAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddTeacherResume_Base")]
        public async Task<ActionResult<BitResultObject>> AddTeacherResume_Base(AddEditTeacherResumeRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            TeacherResume TeacherResume = new TeacherResume()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                DateAchieved = requestBody.DateAchieved.StringToDate(),
                Description = requestBody.Description ?? "",
                Title = requestBody.Title,
                UserId = requestBody.UserId,
            };
            var result = await _TeacherResumeRep.AddTeacherResumeAsync(TeacherResume);
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

        [HttpPut("EditTeacherResume_Base")]
        public async Task<ActionResult<BitResultObject>> EditTeacherResume_Base(AddEditTeacherResumeRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _TeacherResumeRep.GetTeacherResumeByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            TeacherResume TeacherResume = new TeacherResume()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                DateAchieved = requestBody.DateAchieved.StringToDate(),
                Description = requestBody.Description ?? "",
                Title = requestBody.Title,
                UserId = requestBody.UserId,
            };
            result = await _TeacherResumeRep.EditTeacherResumeAsync(TeacherResume);
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

        [HttpDelete("DeleteTeacherResume_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteTeacherResume_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TeacherResumeRep.RemoveTeacherResumeAsync(requestBody.ID);
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
