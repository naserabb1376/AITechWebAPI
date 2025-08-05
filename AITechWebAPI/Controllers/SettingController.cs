using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Setting;
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
using AutoMapper;
using AITechWebAPI.ViewModels;

namespace AITechWebAPI.Controllers
{
    [Route("Setting")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class SettingController : ControllerBase
    {
        ISettingRep _SettingRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public SettingController(ISettingRep SettingRep,ILogRep logRep,IMapper mapper)
        {
           _SettingRep = SettingRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("GetAllSettings_Base")]
        public async Task<ActionResult<SettingListCustomResponse<SettingVM>>> GetAllSettings_Base(GetSettingListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SettingRep.GetAllSettingsAsync(requestBody.ParentId,requestBody.Key,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<SettingListCustomResponse<SettingVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("GetSettingById_Base")]
        public async Task<ActionResult<SettingRowCustomResponse<SettingVM>>> GetSettingById_Base(GetSettingRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SettingRep.GetSettingRowAsync(requestBody.ID,requestBody.Key);
            if (result.Status)
            {
                var resultVM = _mapper.Map<SettingRowCustomResponse<SettingVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistSetting_Base")]
        public async Task<ActionResult<BitResultObject>> ExistSetting_Base(GetSettingRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SettingRep.ExistSettingRowAsync(requestBody.ID,requestBody.Key);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddSetting_Base")]
        public async Task<ActionResult<BitResultObject>> AddSetting_Base(AddEditSettingRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Setting Setting = new Setting()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Key = requestBody.Key,
                Value = requestBody.Value,
                ParentId = requestBody.ParentId,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            var result = await _SettingRep.AddSettingAsync(Setting);
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

        [HttpPut("EditSetting_Base")]
        public async Task<ActionResult<BitResultObject>> EditSetting_Base(AddEditSettingRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _SettingRep.GetSettingRowAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Setting Setting = new Setting()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                Key = requestBody.Key,
                Value = requestBody.Value,
                ParentId = requestBody.ParentId,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _SettingRep.EditSettingAsync(Setting);
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

        [HttpDelete("DeleteSetting_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteSetting_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SettingRep.RemoveSettingAsync(requestBody.ID);
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
