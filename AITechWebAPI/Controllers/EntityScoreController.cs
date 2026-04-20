using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.EntityScore;
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
using System.Net;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("EntityScore")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class EntityScoreController : ControllerBase
    {
        IEntityScoreRep _EntityScoreRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public EntityScoreController(IEntityScoreRep EntityScoreRep,ILogRep logRep,IMapper mapper)
        {
           _EntityScoreRep = EntityScoreRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllEntityScores_Base")]
        public async Task<ActionResult<ListResultObject<EntityScore>>> GetAllEntityScores_Base(GetEntityScoreListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EntityScoreRep.GetAllEntityScoresAsync(requestBody.ForeignKeyId,requestBody.EntityType,requestBody.ScoreItemKey,requestBody.ParentId,requestBody.UserId,requestBody.RecordLevel,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<EntityScore>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetEntityScoreById_Base")]
        public async Task<ActionResult<RowResultObject<EntityScore>>> GetEntityScoreById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EntityScoreRep.GetEntityScoreByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<EntityScore>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistEntityScore_Base")]
        public async Task<ActionResult<BitResultObject>> ExistEntityScore_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EntityScoreRep.ExistEntityScoreAsync(requestBody.ID); ;
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddEntityScore_Base")]
        public async Task<ActionResult<BitResultObject>> AddEntityScore_Base(AddEditEntityScoreRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            EntityScore EntityScore = new EntityScore()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                TargetObjName = requestBody.TargetObjName,
                ScoreItemTitle = requestBody.ScoreItemTitle,
                RecordLevel = requestBody.RecordLevel,
                ScoreItemKey = requestBody.ScoreItemKey.Replace(" ", "").ToLower(),
                EntityType = (requestBody.EntityType ?? "").ToLower(),
                ScoreItemParentId = requestBody.ParentId,
                ForeignKeyId = requestBody.ForeignKeyId ?? 0,
                ScoreItemRawScore = requestBody.ScoreItemRawScore ?? 0.0f,
                ScoreItemWeightPercent = requestBody.ScoreItemWeightPercent ?? 0.0f,
                UserId = requestBody.UserID ?? User.GetCurrentUserId(),
                CreatorId = requestBody.CreatorId ?? User.GetCurrentUserId(),
                ScoreItemTotalScore = 0.0f,
                ScoreItemWeightedScore = 0.0f,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
                Description = requestBody.Description ?? "",
            };
            var result = await _EntityScoreRep.AddEntityScoreAsync(EntityScore);
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

        [HttpPut("EditEntityScore_Base")]
        public async Task<ActionResult<BitResultObject>> EditEntityScore_Base(AddEditEntityScoreRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _EntityScoreRep.GetEntityScoreByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            EntityScore EntityScore = new EntityScore()
            {
                CreateDate = theRow.Result.CreateDate,
                ID = requestBody.ID,
                UpdateDate = DateTime.Now.ToShamsi(),
                TargetObjName = requestBody.TargetObjName,
                ScoreItemTitle = requestBody.ScoreItemTitle,
                RecordLevel = requestBody.RecordLevel,
                ScoreItemKey = requestBody.ScoreItemKey.Replace(" ","").ToLower(),
                EntityType = (requestBody.EntityType ?? "").ToLower(),
                ScoreItemParentId = requestBody.ParentId,
                ForeignKeyId = requestBody.ForeignKeyId ?? 0,
                ScoreItemRawScore = requestBody.ScoreItemRawScore ?? 0.0f,
                ScoreItemWeightPercent = requestBody.ScoreItemWeightPercent ?? 0.0f,
                UserId = requestBody.UserID ?? User.GetCurrentUserId(),
                CreatorId = requestBody.CreatorId ?? User.GetCurrentUserId(),
                ScoreItemTotalScore = 0.0f,
                ScoreItemWeightedScore = 0.0f,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
                Description = requestBody.Description ?? "",
            };
            result = await _EntityScoreRep.EditEntityScoreAsync(EntityScore);
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

      

        [HttpDelete("DeleteEntityScore_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteEntityScore_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EntityScoreRep.RemoveEntityScoreAsync(requestBody.ID);
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
