using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.EducationalBackground;
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
    [Route("EducationalBackground")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class EducationalBackgroundController : ControllerBase
    {
        IEducationalBackgroundRep _EducationalBackgroundRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public EducationalBackgroundController(IEducationalBackgroundRep EducationalBackgroundRep,ILogRep logRep,IMapper mapper)
        {
           _EducationalBackgroundRep = EducationalBackgroundRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllEducationalBackgrounds_Base")]
        public async Task<ActionResult<ListResultObject<EducationalBackgroundVM>>> GetAllEducationalBackground_Base(GetEducationalBackgroundListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EducationalBackgroundRep.GetAllEducationalBackgroundsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<EducationalBackgroundVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetEducationalBackgroundById_Base")]
        public async Task<ActionResult<RowResultObject<EducationalBackgroundVM>>> GetEducationalBackgroundById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EducationalBackgroundRep.GetEducationalBackgroundByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<EducationalBackgroundVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistEducationalBackground_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> ExistEducationalBackground_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EducationalBackgroundRep.ExistEducationalBackgroundAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddEducationalBackground_Base")]
        public async Task<ActionResult<BitResultObject>> AddEducationalBackground_Base(AddEditEducationalBackgroundRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            EducationalBackground EducationalBackground = new EducationalBackground()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = requestBody.UserId,
               // Description = requestBody.Description,
            };
            var result = await _EducationalBackgroundRep.AddEducationalBackgroundAsync(EducationalBackground);
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

        [HttpPut("EditEducationalBackground_Base")]
        public async Task<ActionResult<BitResultObject>> EditEducationalBackground_Base(AddEditEducationalBackgroundRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _EducationalBackgroundRep.GetEducationalBackgroundByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            EducationalBackground EducationalBackground = new EducationalBackground()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = theRow.Result.UserId,
                ID = requestBody.ID,
                // Description = requestBody.Description,


            };
            result = await _EducationalBackgroundRep.EditEducationalBackgroundAsync(EducationalBackground);
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

        [HttpDelete("DeleteEducationalBackground_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteEducationalBackground_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _EducationalBackgroundRep.RemoveEducationalBackgroundAsync(requestBody.ID);
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
