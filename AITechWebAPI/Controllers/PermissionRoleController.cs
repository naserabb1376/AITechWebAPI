using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PermissionRole;
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
    [Route("PermissionRole")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.GeneralAdmin })]

    public class PermissionRoleController : ControllerBase
    {
        IPermissionRoleRep _PermissionRoleRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PermissionRoleController(IPermissionRoleRep PermissionRoleRep,ILogRep logRep,IMapper mapper)
        {
           _PermissionRoleRep = PermissionRoleRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllPermissionRoles_Base")]
        public async Task<ActionResult<ListResultObject<PermissionRoleVM>>> GetAllPermissionRoles_Base(GetPermissionRoleListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PermissionRoleRep.GetAllPermissionRolesAsync(requestBody.RoleId, requestBody.PermissionId, requestBody.PermissionType ?? "",requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<PermissionRoleVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetPermissionRoleById_Base")]
        public async Task<ActionResult<RowResultObject<PermissionRoleVM>>> GetPermissionRoleById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PermissionRoleRep.GetPermissionRoleByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<PermissionRoleVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistPermissionRole_Base")]
        public async Task<ActionResult<BitResultObject>> ExistPermissionRole_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PermissionRoleRep.ExistPermissionRoleAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddPermissionRoles_Base")]
        public async Task<ActionResult<BitResultObject>> AddPermissionRoles_Base(List<AddEditPermissionRoleRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var PermissionRoles = requestBody.Select(x=> new PermissionRole()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                PerrmissionId = x.PermissionId,
                RoleId = x.RoleId,
            }).ToList();
            
            var result = await _PermissionRoleRep.AddPermissionRolesAsync(PermissionRoles);
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

        [HttpPut("EditPermissionRoles_Base")]
        public async Task<ActionResult<BitResultObject>> EditPermissionRoles_Base(List<AddEditPermissionRoleRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var PermissionRoles = new List<PermissionRole>();

            foreach (var body in requestBody)
            {
                var theRow = await _PermissionRoleRep.GetPermissionRoleByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var PermissionRole = new PermissionRole
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    PerrmissionId = body.PermissionId,
                    RoleId = body.RoleId,
                };

                PermissionRoles.Add(PermissionRole);
            }

            result = await _PermissionRoleRep.EditPermissionRolesAsync(PermissionRoles);
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

        [HttpDelete("DeletePermissionRoles_Base")]
        public async Task<ActionResult<BitResultObject>> DeletePermissionRoles_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _PermissionRoleRep.RemovePermissionRolesAsync(ids);
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
