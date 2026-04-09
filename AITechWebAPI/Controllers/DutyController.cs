using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Duty;
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
    [Route("Duty")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class DutyController : ControllerBase
    {
        IDutyRep _DutyRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public DutyController(IDutyRep DutyRep, ILogRep logRep, IMapper mapper)
        {
            _DutyRep = DutyRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllDuties_Base")]
        public async Task<ActionResult<ListResultObject<DutyVM>>> GetAllDuties_Base(GetDutyListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DutyRep.GetAllDutiesAsync(requestBody.UserId, requestBody.SenderUserId, requestBody.ReadState, requestBody.DoneState, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<DutyVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetDutyById_Base")]
        public async Task<ActionResult<RowResultObject<DutyVM>>> GetDutyById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DutyRep.GetDutyByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<DutyVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistDuty_Base")]
        public async Task<ActionResult<BitResultObject>> ExistDuty_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DutyRep.ExistDutyAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddDuty_Base")]
        public async Task<ActionResult<BitResultObject>> AddDuty_Base(AddEditDutyRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Duty Duty = new Duty()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserID,
                DutyTitle = requestBody.DutyTitle,
                DutyDescription = requestBody.DutyDescription,
                IsRead = requestBody.DutyIsRead,
                IsDone = requestBody.DutyIsDone,
                DutyPassLevel = requestBody.DutyPassLevel,
                SenderUserId = requestBody.SenderUserID ?? User.GetCurrentUserId(),
                DutyReport = requestBody.DutyReport ?? "",
                DutyScore = requestBody.DutyScore.Value,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _DutyRep.AddDutyAsync(Duty);
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

        [HttpPut("EditDuty_Base")]
        public async Task<ActionResult<BitResultObject>> EditDuty_Base(AddEditDutyRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _DutyRep.GetDutyByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Duty Duty = new Duty()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserID,
                DutyTitle = requestBody.DutyTitle,
                DutyDescription = requestBody.DutyDescription,
                IsRead = requestBody.DutyIsRead,
                IsDone = requestBody.DutyIsDone,
                DutyPassLevel = requestBody.DutyPassLevel,
                SenderUserId = requestBody.SenderUserID ?? User.GetCurrentUserId(),
                DutyReport = requestBody.DutyReport ?? "",
                DutyScore = requestBody.DutyScore.Value,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _DutyRep.EditDutyAsync(Duty);
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

        [HttpDelete("DeleteDuty_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteDuty_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DutyRep.RemoveDutyAsync(requestBody.ID);
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
    

          // Chart

        [HttpPost("GetDutyChart")]
        public async Task<IActionResult> GetDutyChart(GetDutyChartRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            switch (requestBody.ChartType)
            {
                default:
                case 1:
                    {
                        var result = await _DutyRep.GetEmployeePerformanceChartAsync(requestBody.FromDate, requestBody.ToDate, requestBody.UserId);
                        if (result.Status)
                        {
                            return Ok(result);
                        }
                        return BadRequest(result);
                    }
                case 2:
                    {
                        var result = await _DutyRep.GetDutyTrendChartAsync(requestBody.FromDate, requestBody.ToDate, requestBody.UserId);
                        if (result.Status)
                        {
                            return Ok(result);
                        }
                        return BadRequest(result);
                    }
            }
        }
    }
}
