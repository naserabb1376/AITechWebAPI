using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Software;
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
    [Route("Software")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]


    public class SoftwareController : ControllerBase
    {
        ISoftwareRep _SoftwareRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public SoftwareController(ISoftwareRep SoftwareRep,ILogRep logRep,IMapper mapper)
        {
           _SoftwareRep = SoftwareRep;
           _logRep = logRep;
           _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("GetAllSoftwares_Base")]
        public async Task<ActionResult<SoftwareListCustomResponse<SoftwareVM>>> GetAllSoftwares_Base(GetSoftwareListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SoftwareRep.GetAllSoftwaresAsync(requestBody.CategoryId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<SoftwareListCustomResponse<SoftwareVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetSoftwareById_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<SoftwareRowCustomResponse<SoftwareVM>>> GetSoftwareById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SoftwareRep.GetSoftwareByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<SoftwareRowCustomResponse<SoftwareVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistSoftware_Base")]
        public async Task<ActionResult<BitResultObject>> ExistSoftware_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SoftwareRep.ExistSoftwareAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddSoftware_Base")]
        public async Task<ActionResult<BitResultObject>> AddSoftware_Base(AddEditSoftwareRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Software Software = new Software()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                DownloadUrl = requestBody.DownloadUrl ?? "",
                CategoryId = requestBody.CategoryId,
                Title = requestBody.Title,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",
                

            };
            var result = await _SoftwareRep.AddSoftwareAsync(Software);
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

        [HttpPut("EditSoftware_Base")]
        public async Task<ActionResult<BitResultObject>> EditSoftware_Base(AddEditSoftwareRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _SoftwareRep.GetSoftwareByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Software Software = new Software()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                Description = requestBody.Description ?? "",
                Note = requestBody.Note ?? "",
                DownloadUrl = requestBody.DownloadUrl ?? "",
                CategoryId = requestBody.CategoryId,
                Title = requestBody.Title,
                IsActive = requestBody.IsActive,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _SoftwareRep.EditSoftwareAsync(Software);
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

        [HttpDelete("DeleteSoftware_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteSoftware_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SoftwareRep.RemoveSoftwareAsync(requestBody.ID);
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

