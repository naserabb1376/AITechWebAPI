using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.SessionAssignment;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using AITechWebAPI.ViewModels;

namespace AITechWebAPI.Controllers
{
    [Route("SessionAssignment")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class SessionAssignmentController : ControllerBase
    {
        ISessionAssignmentRep _SessionAssignmentRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;

        public SessionAssignmentController(ISessionAssignmentRep SessionAssignmentRep,ILogRep logRep,IMapper mapper)
        {
           _SessionAssignmentRep = SessionAssignmentRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllSessionAssignments_Base")]
        public async Task<ActionResult<ListResultObject<SessionAssignmentVM>>> GetAllSessionAssignments_Base(GetSessionAssignmentListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionAssignmentRep.GetAllSessionAssignmentsAsync(requestBody.SessionId,requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<SessionAssignmentVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetSessionAssignmentById_Base")]
        public async Task<ActionResult<RowResultObject<SessionAssignmentVM>>> GetSessionAssignmentById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionAssignmentRep.GetSessionAssignmentByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<SessionAssignmentVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistSessionAssignment_Base")]
        public async Task<ActionResult<BitResultObject>> ExistSessionAssignment_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionAssignmentRep.ExistSessionAssignmentAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddSessionAssignment_Base")]
        public async Task<ActionResult<BitResultObject>> AddSessionAssignment_Base(AddEditSessionAssignmentRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            SessionAssignment SessionAssignment = new SessionAssignment()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Description = requestBody.Description ??"",
                DueDate = requestBody.DueDate.StringToDate(),
                Title = requestBody.Title,
                SessionId = requestBody.SessionId,
            };
            var result = await _SessionAssignmentRep.AddSessionAssignmentAsync(SessionAssignment);
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

        [HttpPut("EditSessionAssignment_Base")]
        public async Task<ActionResult<BitResultObject>> EditSessionAssignment_Base(AddEditSessionAssignmentRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _SessionAssignmentRep.GetSessionAssignmentByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            SessionAssignment SessionAssignment = new SessionAssignment()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                Description = requestBody.Description ?? "",
                DueDate = requestBody.DueDate.StringToDate(),
                Title = requestBody.Title,
                SessionId = requestBody.SessionId,
            };
            result = await _SessionAssignmentRep.EditSessionAssignmentAsync(SessionAssignment);
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

        [HttpDelete("DeleteSessionAssignment_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteSessionAssignment_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SessionAssignmentRep.RemoveSessionAssignmentAsync(requestBody.ID);
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
