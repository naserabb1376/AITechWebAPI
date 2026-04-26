using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Dismissal;
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
    [Route("Dismissal")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class DismissalController : ControllerBase
    {
        IDismissalRep _DismissalRep;
        IUserRep _UserRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public DismissalController(IDismissalRep DismissalRep,IUserRep userRep, ILogRep logRep, IMapper mapper)
        {
            _DismissalRep = DismissalRep;
            _UserRep = userRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllDismissals_Base")]
        public async Task<ActionResult<ListResultObject<DismissalVM>>> GetAllDismissals_Base(GetDismissalListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DismissalRep.GetAllDismissalsAsync(requestBody.UserId, requestBody.CheckerUserId, requestBody.ApproveState, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<DismissalVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetDismissalById_Base")]
        public async Task<ActionResult<RowResultObject<DismissalVM>>> GetDismissalById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DismissalRep.GetDismissalByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<DismissalVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistDismissal_Base")]
        public async Task<ActionResult<BitResultObject>> ExistDismissal_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DismissalRep.ExistDismissalAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddDismissal_Base")]
        public async Task<ActionResult<BitResultObject>> AddDismissal_Base(AddEditDismissalRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Dismissal Dismissal = new Dismissal()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserID,
                CheckerUserId = requestBody.CheckerUserID ?? User.GetCurrentUserId(),
                DismissalType = requestBody.DismissalType,
                DismissalRequestDescription = requestBody.DismissalRequestDescription,
                CheckerDescription = requestBody.DismissalCheckDescription,
                IsApproved = requestBody.DismissalApproved,
                DismissalRequestStartDate = requestBody.DismissalRequestStartDate.StringToDate().Value,
                DismissalRequestEndDate = requestBody.DismissalRequestEndDate.StringToDate().Value,
                DismissalApprovedStartDate = requestBody.DismissalApprovedStartDate.StringToDate(),
                DismissalApprovedEndDate = requestBody.DismissalApprovedEndDate.StringToDate(),
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _DismissalRep.AddDismissalAsync(Dismissal);
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

        [HttpPut("EditDismissal_Base")]
        public async Task<ActionResult<BitResultObject>> EditDismissal_Base(AddEditDismissalRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _DismissalRep.GetDismissalByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Dismissal Dismissal = new Dismissal()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserID,
                CheckerUserId = requestBody.CheckerUserID ?? User.GetCurrentUserId(),
                DismissalType = requestBody.DismissalType,
                DismissalRequestDescription = requestBody.DismissalRequestDescription,
                CheckerDescription = requestBody.DismissalCheckDescription,
                IsApproved = requestBody.DismissalApproved,
                DismissalRequestStartDate = requestBody.DismissalRequestStartDate.StringToDate().Value,
                DismissalRequestEndDate = requestBody.DismissalRequestEndDate.StringToDate().Value,
                DismissalApprovedStartDate = requestBody.DismissalApprovedStartDate.StringToDate(),
                DismissalApprovedEndDate = requestBody.DismissalApprovedEndDate.StringToDate(),
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _DismissalRep.EditDismissalAsync(Dismissal);
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

        [HttpDelete("DeleteDismissal_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteDismissal_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DismissalRep.RemoveDismissalAsync(requestBody.ID);
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
