using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.DiscountTarget;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MTPermissionCenter.EFCore.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("DiscountTarget")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.GeneralAdmin })]

    public class DiscountTargetController : ControllerBase
    {
        IDiscountTargetRep _DiscountTargetRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public DiscountTargetController(IDiscountTargetRep DiscountTargetRep,ILogRep logRep,IMapper mapper)
        {
           _DiscountTargetRep = DiscountTargetRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllDiscountTargets_Base")]
        public async Task<ActionResult<ListResultObject<DiscountTargetVM>>> GetAllDiscountTargets_Base(GetDiscountTargetListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountTargetRep.GetAllDiscountTargetsAsync(requestBody.DiscountId, requestBody.TargetId, requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<DiscountTargetVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetDiscountTargetById_Base")]
        public async Task<ActionResult<RowResultObject<DiscountTargetVM>>> GetDiscountTargetById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountTargetRep.GetDiscountTargetByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<DiscountTargetVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistDiscountTarget_Base")]
        public async Task<ActionResult<BitResultObject>> ExistDiscountTarget_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountTargetRep.ExistDiscountTargetAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddDiscountTargets_Base")]
        public async Task<ActionResult<BitResultObject>> AddDiscountTargets_Base(List<AddEditDiscountTargetRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var DiscountTargets = requestBody.Select(x=> new DiscountTarget()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                TargetEntityName = x.TargetEntityName,
                DiscountId = x.DiscountId,
                TargetId = x.TargetId,
                IsActive = x.IsActive,
                
            }).ToList();
            
            var result = await _DiscountTargetRep.AddDiscountTargetsAsync(DiscountTargets);
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

        [HttpPut("EditDiscountTargets_Base")]
        public async Task<ActionResult<BitResultObject>> EditDiscountTargets_Base(List<AddEditDiscountTargetRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var DiscountTargets = new List<DiscountTarget>();

            foreach (var body in requestBody)
            {
                var theRow = await _DiscountTargetRep.GetDiscountTargetByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var DiscountTarget = new DiscountTarget
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    DiscountId = body.DiscountId,
                    TargetId = body.TargetId,
                    TargetEntityName = body.TargetEntityName,
                    IsActive = body.IsActive,
                };

                DiscountTargets.Add(DiscountTarget);
            }

            result = await _DiscountTargetRep.EditDiscountTargetsAsync(DiscountTargets);
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

        [HttpDelete("DeleteDiscountTargets_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteDiscountTargets_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _DiscountTargetRep.RemoveDiscountTargetsAsync(ids);
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
