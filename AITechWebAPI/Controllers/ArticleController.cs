using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Article;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AITechWebAPI.Models.News;
using AITechDATA.CustomResponses;
using AITechWebAPI.Validations;
using AutoMapper;
using AITechWebAPI.ViewModels;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("Article")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]


    public class ArticleController : ControllerBase
    {
        IArticleRep _ArticleRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public ArticleController(IArticleRep ArticleRep,ILogRep logRep,IMapper mapper)
        {
           _ArticleRep = ArticleRep;
           _logRep = logRep;
           _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("GetAllArticles_Base")]
        public async Task<ActionResult<ArticleListCustomResponse<ArticleVM>>> GetAllArticles_Base(GetArticleListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ArticleRep.GetAllArticlesAsync(requestBody.CategoryId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ArticleListCustomResponse<ArticleVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetArticleById_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<ArticleRowCustomResponse<ArticleVM>>> GetArticleById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ArticleRep.GetArticleByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ArticleRowCustomResponse<ArticleVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistArticle_Base")]
        public async Task<ActionResult<BitResultObject>> ExistArticle_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ArticleRep.ExistArticleAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddArticle_Base")]
        public async Task<ActionResult<BitResultObject>> AddArticle_Base(AddEditArticleRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Article Article = new Article()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                CategoryId = requestBody.CategoryId,
                Title = requestBody.Title,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            var result = await _ArticleRep.AddArticleAsync(Article);
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

        [HttpPut("EditArticle_Base")]
        public async Task<ActionResult<BitResultObject>> EditArticle_Base(AddEditArticleRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _ArticleRep.GetArticleByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Article Article = new Article()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                CategoryId = requestBody.CategoryId,
                Title = requestBody.Title,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _ArticleRep.EditArticleAsync(Article);
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

        [HttpDelete("DeleteArticle_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteArticle_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _ArticleRep.RemoveArticleAsync(requestBody.ID);
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
