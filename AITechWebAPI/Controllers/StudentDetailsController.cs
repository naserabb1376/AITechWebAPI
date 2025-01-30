using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.StudentDetails;
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
    [Route("StudentDetails")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class StudentDetailsController : ControllerBase
    {
        IStudentDetailsRep _StudentDetailsRep;
        ILogRep _logRep;

        public StudentDetailsController(IStudentDetailsRep StudentDetailsRep,ILogRep logRep)
        {
           _StudentDetailsRep = StudentDetailsRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllStudentDetails_Base")]
        public async Task<ActionResult<ListResultObject<StudentDetails>>> GetAllStudentDetails_Base(GetStudentDetailsListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.GetAllStudentDetailsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetStudentDetailsById_Base")]
        public async Task<ActionResult<RowResultObject<StudentDetails>>> GetStudentDetailsById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.GetStudentDetailsByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistStudentDetails_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> ExistStudentDetails_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.ExistStudentDetailsAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddStudentDetails_Base")]
        public async Task<ActionResult<BitResultObject>> AddStudentDetails_Base(AddEditStudentDetailsRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            StudentDetails StudentDetails = new StudentDetails()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserId,
               // Description = requestBody.Description,
            };
            var result = await _StudentDetailsRep.AddStudentDetailsAsync(StudentDetails);
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

        [HttpPut("EditStudentDetails_Base")]
        public async Task<ActionResult<BitResultObject>> EditStudentDetails_Base(AddEditStudentDetailsRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _StudentDetailsRep.GetStudentDetailsByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            StudentDetails StudentDetails = new StudentDetails()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = theRow.Result.UserId,
                ID = requestBody.ID,
                // Description = requestBody.Description,


            };
            result = await _StudentDetailsRep.EditStudentDetailsAsync(StudentDetails);
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

        [HttpDelete("DeleteStudentDetails_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteStudentDetails_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _StudentDetailsRep.RemoveStudentDetailsAsync(requestBody.ID);
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
