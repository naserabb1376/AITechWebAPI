using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.LinkedEntity;
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
    [Route("LinkedEntity")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class LinkedEntityController : ControllerBase
    {
        private ILinkedEntityRep _LinkedEntityRep;
        private ILogRep _logRep;

        public LinkedEntityController(ILinkedEntityRep LinkedEntityRep, ILogRep logRep)
        {
            _LinkedEntityRep = LinkedEntityRep;
            _logRep = logRep;
        }

        [HttpPost("GetAllLinkedEntities_Base")]
        public async Task<ActionResult<ListResultObject<LinkedEntity>>> GetAllLinkedEntities_Base(GetLinkedEntityListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LinkedEntityRep.GetAllLinkedEntitiesAsync(requestBody.EntityName, requestBody.ForeignKeyId,requestBody.LinkedEntityId, requestBody.CreatorId, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetLinkedEntityById_Base")]
        public async Task<ActionResult<RowResultObject<LinkedEntity>>> GetLinkedEntityById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LinkedEntityRep.GetLinkedEntityByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistLinkedEntity_Base")]
        public async Task<ActionResult<BitResultObject>> ExistLinkedEntity_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LinkedEntityRep.ExistLinkedEntityAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddLinkedEntity_Base")]
        public async Task<ActionResult<BitResultObject>> AddLinkedEntity_Base(AddEditLinkedEntityRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            LinkedEntity LinkedEntity = new LinkedEntity()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                ForeignKeyId = requestBody.ForeignKeyId,
                EntityName = requestBody.EntityName,
                LinkedEntityId = requestBody.LinkedEntityId,
                Priority = requestBody.Priority,
                LinkType = requestBody.LinkType,
                CreatorId = requestBody.CreatorId ?? 0,
                Description = requestBody.Description ?? "",

            };
            var result = await _LinkedEntityRep.AddLinkedEntityAsync(LinkedEntity);
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

        [HttpPut("EditLinkedEntity_Base")]
        public async Task<ActionResult<BitResultObject>> EditLinkedEntity_Base(AddEditLinkedEntityRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _LinkedEntityRep.GetLinkedEntityByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            LinkedEntity LinkedEntity = new LinkedEntity()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                ForeignKeyId = requestBody.ForeignKeyId,
                EntityName = requestBody.EntityName,
                LinkedEntityId = requestBody.LinkedEntityId,
                Priority = requestBody.Priority,
                LinkType = requestBody.LinkType,
                CreatorId = requestBody.CreatorId ?? 0,
                Description = requestBody.Description ?? "",

            };
            result = await _LinkedEntityRep.EditLinkedEntityAsync(LinkedEntity);
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

        [HttpDelete("DeleteLinkedEntity_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteLinkedEntity_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LinkedEntityRep.RemoveLinkedEntityAsync(requestBody.ID);
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