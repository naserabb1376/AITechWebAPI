using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.UserGroup;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AITechWebAPI.Controllers
{
    [Route("UserGroup")]
    [ApiController]
    //[Authorize]
    [Produces("application/json")]

    public class UserGroupController : ControllerBase
    {
        IUserGroupRep _UserGroupRep;
        ILogRep _logRep;

        public UserGroupController(IUserGroupRep UserGroupRep,ILogRep logRep)
        {
           _UserGroupRep = UserGroupRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllUserGroups_Base")]
        public async Task<ActionResult<ListResultObject<UserGroup>>> GetAllUserGroups_Base(GetUserGroupListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserGroupRep.GetAllUserGroupsAsync(requestBody.UserId,requestBody.GroupId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetUserGroupById_Base")]
        public async Task<ActionResult<RowResultObject<UserGroup>>> GetUserGroupById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserGroupRep.GetUserGroupByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistUserGroup_Base")]
        public async Task<ActionResult<BitResultObject>> ExistUserGroup_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserGroupRep.ExistUserGroupAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddUserGroups_Base")]
        public async Task<ActionResult<BitResultObject>> AddUserGroups_Base(List<AddEditUserGroupRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var UserGroups = requestBody.Select(x=> new UserGroup()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = x.UserId,
                GroupId = x.GroupId,
            }).ToList();
            
            var result = await _UserGroupRep.AddUserGroupsAsync(UserGroups);
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

        [HttpPut("EditUserGroups_Base")]
        public async Task<ActionResult<BitResultObject>> EditUserGroups_Base(List<AddEditUserGroupRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var UserGroups = new List<UserGroup>();

            foreach (var body in requestBody)
            {
                var theRow = await _UserGroupRep.GetUserGroupByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var UserGroup = new UserGroup
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    UserId = body.UserId,
                    GroupId = body.GroupId,
                };

                UserGroups.Add(UserGroup);
            }

            result = await _UserGroupRep.EditUserGroupsAsync(UserGroups);
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

        [HttpDelete("DeleteUserGroups_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteUserGroups_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _UserGroupRep.RemoveUserGroupsAsync(ids);
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
