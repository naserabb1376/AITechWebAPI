using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Assignment;
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
    [Route("Assignment")]
    [ApiController]
    //[Authorize]
    [Produces("application/json")]

    public class AssignmentController : ControllerBase
    {
        IAssignmentRep _AssignmentRep;
        ILogRep _logRep;

        public AssignmentController(IAssignmentRep AssignmentRep,ILogRep logRep)
        {
           _AssignmentRep = AssignmentRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllAssignments_Base")]
        public async Task<ActionResult<ListResultObject<Assignment>>> GetAllAssignments_Base(GetAssignmentListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AssignmentRep.GetAllAssignmentsAsync(requestBody.UserId,requestBody.SessionAssignmentId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetAssignmentById_Base")]
        public async Task<ActionResult<RowResultObject<Assignment>>> GetAssignmentById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AssignmentRep.GetAssignmentByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistAssignment_Base")]
        public async Task<ActionResult<BitResultObject>> ExistAssignment_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AssignmentRep.ExistAssignmentAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddAssignments_Base")]
        public async Task<ActionResult<BitResultObject>> AddAssignments_Base(List<AddEditAssignmentRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var Assignments = requestBody.Select(x=> new Assignment()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Description = x.Description ?? "",
                SubmissionDate = x.SubmissionDate.StringToDate(),
                SessionAssignmentId = x.SessionAssignmentId,
                Title = x.Title,
                UserId = x.UserId,
            }).ToList();
            
            var result = await _AssignmentRep.AddAssignmentsAsync(Assignments);
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

        [HttpPut("EditAssignments_Base")]
        public async Task<ActionResult<BitResultObject>> EditAssignments_Base(List<AddEditAssignmentRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var Assignments = new List<Assignment>();

            foreach (var body in requestBody)
            {
                var theRow = await _AssignmentRep.GetAssignmentByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var Assignment = new Assignment
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    Description = body.Description ?? "",
                    SessionAssignmentId = body.SessionAssignmentId,
                    SubmissionDate = body.SubmissionDate.StringToDate(),
                    UserId = body.UserId,
                    Title = body.Title,
                };

                Assignments.Add(Assignment);
            }

            result = await _AssignmentRep.EditAssignmentsAsync(Assignments);
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

        [HttpDelete("DeleteAssignments_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteAssignments_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _AssignmentRep.RemoveAssignmentsAsync(ids);
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
