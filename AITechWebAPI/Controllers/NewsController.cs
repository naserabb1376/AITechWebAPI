using AITechDATA.CustomResponses;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.News;
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
    [Route("News")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]


    public class NewsController : ControllerBase
    {
        INewsRep _NewsRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public NewsController(INewsRep NewsRep,ILogRep logRep,IMapper mapper)
        {
           _NewsRep = NewsRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("GetAllNews_Base")]
        public async Task<ActionResult<NewsListCustomResponse<NewsVM>>> GetAllNews_Base(GetNewsListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NewsRep.GetAllNewsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<NewsListCustomResponse<NewsVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("GetNewsById_Base")]
        public async Task<ActionResult<NewsRowCustomResponse<NewsVM>>> GetNewsById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NewsRep.GetNewsByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<NewsRowCustomResponse<NewsVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistNews_Base")]
        public async Task<ActionResult<BitResultObject>> ExistNews_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NewsRep.ExistNewsAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddNews_Base")]
        public async Task<ActionResult<BitResultObject>> AddNews_Base(AddEditNewsRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            News News = new News()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserId,
                Content = requestBody.Content,
                Source = requestBody.Source,
                PublishDate = requestBody.PublishDate.StringToDate().Value,
                Keywords = requestBody.Keywords,
                Title = requestBody.Title,
                Note = requestBody.Note ?? "",
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            var result = await _NewsRep.AddNewsAsync(News);
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

        [HttpPut("EditNews_Base")]
        public async Task<ActionResult<BitResultObject>> EditNews_Base(AddEditNewsRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _NewsRep.GetNewsByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            News News = new News()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                UserId = requestBody.UserId,
                Content = requestBody.Content,
                Source = requestBody.Source,
                PublishDate = requestBody.PublishDate.StringToDate().Value,
                Keywords = requestBody.Keywords,
                Title = requestBody.Title,
                Note = requestBody.Note ?? "",
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _NewsRep.EditNewsAsync(News);
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

        [HttpDelete("DeleteNews_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteNews_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _NewsRep.RemoveNewsAsync(requestBody.ID);
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
