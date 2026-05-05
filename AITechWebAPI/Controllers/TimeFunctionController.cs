using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.TimeFunction;
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
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("TimeFunction")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class TimeFunctionController : ControllerBase
    {
        ITimeFunctionRep _TimeFunctionRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public TimeFunctionController(ITimeFunctionRep TimeFunctionRep, ILogRep logRep, IMapper mapper)
        {
            _TimeFunctionRep = TimeFunctionRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllTimeFunctions")]
        public async Task<ActionResult<ListResultObject<TimeFunctionVM>>> GetAllTimeFunctions(GetTimeFunctionListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeFunctionRep.GetAllTimeFunctionsAsync(requestBody.UserId, requestBody.TimeFunctionStartDate, requestBody.TimeFunctionEndDate, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<TimeFunctionVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetTimeFunctionById")]
        public async Task<ActionResult<RowResultObject<TimeFunctionVM>>> GetTimeFunctionById(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeFunctionRep.GetTimeFunctionByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<TimeFunctionVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistTimeFunction_Base")]
        public async Task<ActionResult<BitResultObject>> ExistTimeFunction_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeFunctionRep.ExistTimeFunctionAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddTimeFunction_Base")]
        public async Task<ActionResult<BitResultObject>> AddTimeFunction_Base(AddEditTimeFunctionRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            TimeFunction TimeFunction = new TimeFunction()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserID ?? User.GetCurrentUserId(),
                Description = requestBody.Description,
                TimeFunctionStartDate = requestBody.TimeFunctionStartDate.StringToDate().Value,
                TimeFunctionEndDate = requestBody.TimeFunctionEndDate.StringToDate().Value,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _TimeFunctionRep.AddTimeFunctionAsync(TimeFunction);
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

        [HttpPut("EditTimeFunction_Base")]
        public async Task<ActionResult<BitResultObject>> EditTimeFunction_Base(AddEditTimeFunctionRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _TimeFunctionRep.GetTimeFunctionByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            TimeFunction TimeFunction = new TimeFunction()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserID ?? User.GetCurrentUserId(),
                Description = requestBody.Description,
                TimeFunctionStartDate = requestBody.TimeFunctionStartDate.StringToDate().Value,
                TimeFunctionEndDate = requestBody.TimeFunctionEndDate.StringToDate().Value,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _TimeFunctionRep.EditTimeFunctionAsync(TimeFunction);
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

        [HttpDelete("DeleteTimeFunction_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteTimeFunction_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeFunctionRep.RemoveTimeFunctionAsync(requestBody.ID);
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
