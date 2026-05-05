using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.TimeBreak;
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
    [Route("TimeBreak")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class TimeBreakController : ControllerBase
    {
        ITimeBreakRep _TimeBreakRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public TimeBreakController(ITimeBreakRep TimeBreakRep, ILogRep logRep, IMapper mapper)
        {
            _TimeBreakRep = TimeBreakRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllTimeBreaks_Base")]
        public async Task<ActionResult<ListResultObject<TimeBreakVM>>> GetAllTimeBreaks_Base(GetTimeBreakListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeBreakRep.GetAllTimeBreaksAsync(requestBody.TimeFunctionID, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<TimeBreakVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetTimeBreakById_Base")]
        public async Task<ActionResult<RowResultObject<TimeBreakVM>>> GetTimeBreakById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeBreakRep.GetTimeBreakByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<TimeBreakVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistTimeBreak_Base")]
        public async Task<ActionResult<BitResultObject>> ExistTimeBreak_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeBreakRep.ExistTimeBreakAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddTimeBreak_Base")]
        public async Task<ActionResult<BitResultObject>> AddTimeBreak_Base(AddEditTimeBreakRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            TimeBreak TimeBreak = new TimeBreak()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                TimeFunctionId = requestBody.TimeFunctionID,
                Description = requestBody.Description,
                TimeBreakStartTime = requestBody.TimeBreakStartTime.StringToTimeSpan(),
                TimeBreakEndTime = requestBody.TimeBreakEndTime.StringToTimeSpan(),
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _TimeBreakRep.AddTimeBreakAsync(TimeBreak);
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

        [HttpPut("EditTimeBreak_Base")]
        public async Task<ActionResult<BitResultObject>> EditTimeBreak_Base(AddEditTimeBreakRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _TimeBreakRep.GetTimeBreakByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            TimeBreak TimeBreak = new TimeBreak()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                TimeFunctionId = requestBody.TimeFunctionID,
                Description = requestBody.Description,
                TimeBreakStartTime = requestBody.TimeBreakStartTime.StringToTimeSpan(),
                TimeBreakEndTime = requestBody.TimeBreakEndTime.StringToTimeSpan(),
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _TimeBreakRep.EditTimeBreakAsync(TimeBreak);
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

        [HttpDelete("DeleteTimeBreak_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteTimeBreak_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _TimeBreakRep.RemoveTimeBreakAsync(requestBody.ID);
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
