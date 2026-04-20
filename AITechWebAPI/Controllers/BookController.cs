using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Book;
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
    [Route("Book")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]


    public class BookController : ControllerBase
    {
        IBookRep _BookRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public BookController(IBookRep BookRep,ILogRep logRep,IMapper mapper)
        {
           _BookRep = BookRep;
           _logRep = logRep;
           _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("GetAllBooks_Base")]
        public async Task<ActionResult<BookListCustomResponse<BookVM>>> GetAllBooks_Base(GetBookListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _BookRep.GetAllBooksAsync(requestBody.CategoryId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<BookListCustomResponse<BookVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetBookById_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BookRowCustomResponse<BookVM>>> GetBookById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _BookRep.GetBookByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<BookRowCustomResponse<BookVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistBook_Base")]
        public async Task<ActionResult<BitResultObject>> ExistBook_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _BookRep.ExistBookAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddBook_Base")]
        public async Task<ActionResult<BitResultObject>> AddBook_Base(AddEditBookRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Book Book = new Book()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                CategoryId = requestBody.CategoryId,
                Title = requestBody.Title,
                AuthorName = requestBody.AuthorName ?? "",
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
                

            };
            var result = await _BookRep.AddBookAsync(Book);
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

        [HttpPut("EditBook_Base")]
        public async Task<ActionResult<BitResultObject>> EditBook_Base(AddEditBookRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _BookRep.GetBookByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Book Book = new Book()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                CategoryId = requestBody.CategoryId,
                Title = requestBody.Title,
                AuthorName = requestBody.AuthorName ?? "",
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _BookRep.EditBookAsync(Book);
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

        [HttpDelete("DeleteBook_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteBook_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _BookRep.RemoveBookAsync(requestBody.ID);
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
