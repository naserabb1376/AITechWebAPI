using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.SubmitForm;
using AITechWebAPI.Tools;
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
    [Route("SubmitForm")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class SubmitFormController : ControllerBase
    {
        private ISubmitFormRep _SubmitFormRep;
        private IFieldInFormRep _FieldInForm;
        private ILogRep _logRep;
        private readonly IMapper _mapper;


        public SubmitFormController(ISubmitFormRep SubmitFormRep,IFieldInFormRep fieldInFormRep, ILogRep logRep,IMapper mapper)
        {
            _SubmitFormRep = SubmitFormRep;
            _FieldInForm = fieldInFormRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllSubmitForms_Base")]
        public async Task<ActionResult<ListResultObject<SubmitForm>>> GetAllSubmitForms_Base(GetSubmitFormListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SubmitFormRep.GetAllSubmitFormsAsync(requestBody.EntityName,requestBody.CreatorId,requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<SubmitFormVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetSubmitFormById_Base")]
        public async Task<ActionResult<RowResultObject<SubmitForm>>> GetSubmitFormById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SubmitFormRep.GetSubmitFormByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<SubmitFormVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetSubmitFormObject")]
        public async Task<ActionResult<RowResultObject<SubmitFormObjDto>>> GetSubmitFormObject(GetSubmitFormObjRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            long recordId = 0;
            if (!requestBody.SearchField.ToLower().Contains("key"))
            {
                recordId = long.Parse(requestBody.SearchValue);
            }

            var result = await _SubmitFormRep.GetSubmitFormObjAsync(recordId,requestBody.SearchValue);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpPost("ExistSubmitForm_Base")]
        public async Task<ActionResult<BitResultObject>> ExistSubmitForm_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SubmitFormRep.ExistSubmitFormAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddSubmitForm")]
        public async Task<ActionResult<BitResultObject>> AddSubmitForm(AddEditSubmitFormRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            SubmitForm SubmitForm = new SubmitForm()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                OtherLangs = requestBody.OtherLangs,
                Description = requestBody.Description,
                IsActive = requestBody.IsActive,
                CreatorId = User.GetCurrentUserId(),
                EntityName = requestBody.EntityName,
                Title = requestBody.FormTitle,
                FormKey = requestBody.FormKey,
                
            };
            var result = await _SubmitFormRep.AddSubmitFormAsync(SubmitForm);
            if (result.Status)
            {
                var fieldInForms = requestBody.FieldIds.Select(x=> new FieldInForm()
                {
                    CreateDate= DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    IsActive = requestBody.IsActive,
                    FormId = result.ID,
                    FormFieldId = x,

                }).ToList();

                result = await _FieldInForm.AddFieldInFormsAsync(fieldInForms);

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

            }
            return BadRequest(result);
        }

        [HttpPut("EditSubmitForm")]
        public async Task<ActionResult<BitResultObject>> EditSubmitForm(AddEditSubmitFormRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _SubmitFormRep.GetSubmitFormByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            SubmitForm SubmitForm = new SubmitForm()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                OtherLangs = requestBody.OtherLangs,
                Description = requestBody.Description,
                IsActive = requestBody.IsActive,
                CreatorId = theRow.Result.CreatorId,
                EntityName = requestBody.EntityName,
                Title = requestBody.FormTitle,
                FormKey = requestBody.FormKey,
            };
            result = await _SubmitFormRep.EditSubmitFormAsync(SubmitForm);
            if (result.Status)
            {
                var fieldInForms = requestBody.FieldIds.Select(x => new FieldInForm()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    IsActive = requestBody.IsActive,
                    FormId = result.ID,
                    FormFieldId = x,

                }).ToList();

                result = await _FieldInForm.AddFieldInFormsAsync(fieldInForms);

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
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteSubmitForm_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteSubmitForm_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SubmitFormRep.RemoveSubmitFormAsync(requestBody.ID);
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