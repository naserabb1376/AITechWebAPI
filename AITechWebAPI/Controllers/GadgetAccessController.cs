using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.GadgetAccess;
using AITechWebAPI.Models.Public;
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

namespace AITechWebAPI.Controllers
{
    [Route("GadgetAccess")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class GadgetAccessController : ControllerBase
    {
        private IGadgetAccessRep _GadgetAccessRep;
        private ILogRep _logRep;
        private readonly IMapper _mapper;

        public GadgetAccessController(IGadgetAccessRep GadgetAccessRep, ILogRep logRep,IMapper mapper)
        {
            _GadgetAccessRep = GadgetAccessRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllGadgetAccesss_Base")]
        public async Task<ActionResult<ListResultObject<GadgetAccessVM>>> GetAllGadgetAccesss_Base(GetGadgetAccessListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GadgetAccessRep.GetAllGadgetAccessesAsync(requestBody.AccessUserName, requestBody.GadgetKey,requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<GadgetAccessVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetAccessableGadgets_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<ListResultObject<AccessableGadgetsDto>>> GetAccessableGadgets_Base(GetAccessableGadgetsRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GadgetAccessRep.GetAcessableGadgetsAsync(requestBody.AccessUserName, requestBody.AccessPassword,requestBody.GadgetKey);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetGadgetAccessById_Base")]
        public async Task<ActionResult<RowResultObject<GadgetAccessVM>>> GetGadgetAccessById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GadgetAccessRep.GetGadgetAccessByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<GadgetAccessVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistGadgetAccess_Base")]
        public async Task<ActionResult<BitResultObject>> ExistGadgetAccess_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GadgetAccessRep.ExistGadgetAccessAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddGadgetAccess_Base")]
        public async Task<ActionResult<RowResultObject<GadgetAccess>>> AddGadgetAccess_Base(AddEditGadgetAccessRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            GadgetAccess GadgetAccess = new GadgetAccess()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                GadgetKey = requestBody.GadgetKey,
                GadgetDescription = requestBody.GadgetDescription,
                GadgetUrl = requestBody.GadgetUrl,
                AccessUsername = requestBody.AccessUserName,
                AccessPassword = requestBody.AccessPassword,
                AccessStartDate = requestBody.AccessStartDate.StringToDate(),
                AccessEndDate = requestBody.AccessEndDate.StringToDate().Value,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            var result = await _GadgetAccessRep.AddGadgetAccessAsync(GadgetAccess);
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

        [HttpPut("EditGadgetAccess_Base")]
        public async Task<ActionResult<RowResultObject<GadgetAccess>>> EditGadgetAccess_Base(AddEditGadgetAccessRequestBody requestBody)
        {
            var result = new RowResultObject<GadgetAccess>();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _GadgetAccessRep.GetGadgetAccessByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            GadgetAccess GadgetAccess = new GadgetAccess()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                GadgetKey = requestBody.GadgetKey,
                GadgetDescription = requestBody.GadgetDescription,
                GadgetUrl = requestBody.GadgetUrl,
                AccessUsername = requestBody.AccessUserName,
                AccessPassword = requestBody.AccessPassword,
                AccessStartDate = requestBody.AccessStartDate.StringToDate(),
                AccessEndDate = requestBody.AccessEndDate.StringToDate().Value,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _GadgetAccessRep.EditGadgetAccessAsync(GadgetAccess);
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

        [HttpDelete("DeleteGadgetAccess_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteGadgetAccess_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _GadgetAccessRep.RemoveGadgetAccessAsync(requestBody.ID);
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