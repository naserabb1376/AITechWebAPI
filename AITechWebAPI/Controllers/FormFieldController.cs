using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.FormField;
using AITechWebAPI.Models.Public;
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

namespace AITechWebAPI.Controllers
{
    [Route("FormField")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class FormFieldController : ControllerBase
    {
        private IFormFieldRep _FormFieldRep;
        private ILogRep _logRep;
        private readonly IMapper _mapper;


        public FormFieldController(IFormFieldRep FormFieldRep, ILogRep logRep,IMapper mapper)
        {
            _FormFieldRep = FormFieldRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllFormFields_Base")]
        public async Task<ActionResult<ListResultObject<FormField>>> GetAllFormFields_Base(GetFormFieldListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FormFieldRep.GetAllFormFieldsAsync(requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<FormFieldVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetFormFieldById_Base")]
        public async Task<ActionResult<RowResultObject<FormField>>> GetFormFieldById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FormFieldRep.GetFormFieldByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<FormFieldVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistFormField_Base")]
        public async Task<ActionResult<BitResultObject>> ExistFormField_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FormFieldRep.ExistFormFieldAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddFormField_Base")]
        public async Task<ActionResult<BitResultObject>> AddFormField_Base(AddEditFormFieldRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            FormField FormField = new FormField()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                FieldName = requestBody.FieldName,
                DisplayName = requestBody.DisplayName,
                OtherLangs = requestBody.OtherLangs,
                Description = requestBody.Description,
                IsActive = requestBody.IsActive,

            };
            var result = await _FormFieldRep.AddFormFieldAsync(FormField);
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

        [HttpPut("EditFormField_Base")]
        public async Task<ActionResult<BitResultObject>> EditFormField_Base(AddEditFormFieldRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _FormFieldRep.GetFormFieldByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            FormField FormField = new FormField()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                FieldName = requestBody.FieldName,
                DisplayName = requestBody.DisplayName,
                OtherLangs = requestBody.OtherLangs,
                Description = requestBody.Description,
                IsActive = requestBody.IsActive,
            };
            result = await _FormFieldRep.EditFormFieldAsync(FormField);
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

        [HttpDelete("DeleteFormField_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteFormField_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FormFieldRep.RemoveFormFieldAsync(requestBody.ID);
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