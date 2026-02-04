using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Attendance;
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
    [Route("Attendance")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class AttendanceController : ControllerBase
    {
        IAttendanceRep _AttendanceRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public AttendanceController(IAttendanceRep AttendanceRep,ILogRep logRep,IMapper mapper)
        {
           _AttendanceRep = AttendanceRep;
           _logRep = logRep;
           _mapper = mapper;
        }

        [HttpPost("GetAllAttendances_Base")]
        public async Task<ActionResult<ListResultObject<AttendanceVM>>> GetAllAttendances_Base(GetAttendanceListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AttendanceRep.GetAllAttendancesAsync(requestBody.UserId,requestBody.SessionId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<AttendanceVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetAttendanceById_Base")]
        public async Task<ActionResult<RowResultObject<AttendanceVM>>> GetAttendanceById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AttendanceRep.GetAttendanceByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<AttendanceVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistAttendance_Base")]
        public async Task<ActionResult<BitResultObject>> ExistAttendance_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AttendanceRep.ExistAttendanceAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddAttendances_Base")]
        public async Task<ActionResult<BitResultObject>> AddAttendances_Base(List<AddEditAttendanceRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var Attendances = requestBody.Select(x=> new Attendance()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                SessionId = x.SessionId,
                IsPresent = x.IsPresent,
                UserId = x.UserId,

            }).ToList();
            
            var result = await _AttendanceRep.AddAttendancesAsync(Attendances);
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

        [HttpPut("EditAttendances_Base")]
        public async Task<ActionResult<BitResultObject>> EditAttendances_Base(List<AddEditAttendanceRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var Attendances = new List<Attendance>();

            foreach (var body in requestBody)
            {
                var theRow = await _AttendanceRep.GetAttendanceByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var Attendance = new Attendance
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    SessionId = body.SessionId,
                    UserId = body.UserId,
                    IsPresent = body.IsPresent,
                };

                Attendances.Add(Attendance);
            }

            result = await _AttendanceRep.EditAttendancesAsync(Attendances);
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

        [HttpDelete("DeleteAttendances_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteAttendances_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _AttendanceRep.RemoveAttendancesAsync(ids);
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
