using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Award;
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
    [Route("Award")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class AwardController : ControllerBase
    {
        IAwardRep _AwardRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public AwardController(IAwardRep AwardRep,ILogRep logRep,IMapper mapper)
        {
           _AwardRep = AwardRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllAwards_Base")]
        public async Task<ActionResult<ListResultObject<Award>>> GetAllAwards_Base(GetAwardListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AwardRep.GetAllAwardsAsync(requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<Award>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetAwardById_Base")]
        public async Task<ActionResult<RowResultObject<Award>>> GetAwardById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AwardRep.GetAwardByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<Award>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistAward_Base")]
        public async Task<ActionResult<BitResultObject>> ExistAward_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AwardRep.ExistAwardAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddAward_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> AddAward_Base(AddEditAwardRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Award Award = new Award()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email ?? "",
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,
                AwardTitle = requestBody.AwardTitle,
                OtherLangs = requestBody.OtherLangs ?? "",
                Description = requestBody.Description ?? "",
            };
            var result = await _AwardRep.AddAwardAsync(Award);
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

        [HttpPut("EditAward_Base")]
        public async Task<ActionResult<BitResultObject>> EditAward_Base(AddEditAwardRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _AwardRep.GetAwardByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Award Award = new Award()
            {
                CreateDate = theRow.Result.CreateDate,
                ID = requestBody.ID,
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email ?? "",
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,
                AwardTitle = requestBody.AwardTitle,
                OtherLangs = requestBody.OtherLangs ?? "",
                Description = requestBody.Description ?? "",
            };
            result = await _AwardRep.EditAwardAsync(Award);
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

      

        [HttpDelete("DeleteAward_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteAward_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AwardRep.RemoveAwardAsync(requestBody.ID);
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
