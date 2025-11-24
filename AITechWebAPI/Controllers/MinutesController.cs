using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Minutes;
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
    [Route("Minutes")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]

    public class MinutesController : ControllerBase
    {
        private IMinutesRep _MinutesRep;
        private readonly IMapper _mapper;
        private ILogRep _logRep;

        public MinutesController(IMinutesRep MinutesRep, ILogRep logRep,IMapper mapper)
        {
            _MinutesRep = MinutesRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllMinutes_Base")]
        public async Task<ActionResult<ListResultObject<MinutesVM>>> GetAllMinutes_Base(GetMinutesListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MinutesRep.GetAllMinutesAsync(requestBody.MeetingId,requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<MinutesVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetMinutesById_Base")]
        public async Task<ActionResult<RowResultObject<MinutesVM>>> GetMinutesById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MinutesRep.GetMinutesByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<MinutesVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

      

        [HttpPost("ExistMinutes_Base")]
        public async Task<ActionResult<BitResultObject>> ExistMinutes_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MinutesRep.ExistMinutesAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddMinutes_Base")]
        public async Task<ActionResult<BitResultObject>> AddMinutes_Base(AddEditMinutesRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Minutes Minutes = new Minutes()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                MeetingId = requestBody.MeetingId,
                MinutesSubject = requestBody.MinutesSubject,
                MinutesDescription = requestBody.MinutesDescription,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _MinutesRep.AddMinutesAsync(Minutes);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("EditMinutes_Base")]
        public async Task<ActionResult<BitResultObject>> EditMinutes_Base(AddEditMinutesRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _MinutesRep.GetMinutesByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Minutes Minutes = new Minutes()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                MeetingId = requestBody.MeetingId,
                MinutesSubject = requestBody.MinutesSubject,
                MinutesDescription = requestBody.MinutesDescription,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _MinutesRep.EditMinutesAsync(Minutes);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteMinutes_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteMinutes_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _MinutesRep.RemoveMinutesAsync(requestBody.ID);
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

                #endregion AddLog

                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}