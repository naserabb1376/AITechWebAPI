using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.FieldInForm;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MTPermissionCenter.EFCore.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("FieldInForm")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.GeneralAdmin })]

    public class FieldInFormController : ControllerBase
    {
        IFieldInFormRep _FieldInFormRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public FieldInFormController(IFieldInFormRep FieldInFormRep,ILogRep logRep,IMapper mapper)
        {
           _FieldInFormRep = FieldInFormRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllFieldInForms_Base")]
        public async Task<ActionResult<ListResultObject<FieldInFormVM>>> GetAllFieldInForms_Base(GetFieldInFormListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FieldInFormRep.GetAllFieldInFormsAsync(requestBody.SubmitFormId, requestBody.FormFieldId, requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<FieldInFormVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetFieldInFormById_Base")]
        public async Task<ActionResult<RowResultObject<FieldInFormVM>>> GetFieldInFormById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FieldInFormRep.GetFieldInFormByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<FieldInFormVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistFieldInForm_Base")]
        public async Task<ActionResult<BitResultObject>> ExistFieldInForm_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _FieldInFormRep.ExistFieldInFormAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddFieldInForms_Base")]
        public async Task<ActionResult<BitResultObject>> AddFieldInForms_Base(List<AddEditFieldInFormRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var FieldInForms = requestBody.Select(x=> new FieldInForm()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                FormId = x.FormId,
                FormFieldId = x.FormFieldId,
                IsActive = x.IsActive,
                
            }).ToList();
            
            var result = await _FieldInFormRep.AddFieldInFormsAsync(FieldInForms);
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

        [HttpPut("EditFieldInForms_Base")]
        public async Task<ActionResult<BitResultObject>> EditFieldInForms_Base(List<AddEditFieldInFormRequestBody> requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var result = new BitResultObject();
            var FieldInForms = new List<FieldInForm>();

            foreach (var body in requestBody)
            {
                var theRow = await _FieldInFormRep.GetFieldInFormByIdAsync(body.ID);
                if (!theRow.Status)
                {
                    result.Status = theRow.Status;
                    result.ErrorMessage = theRow.ErrorMessage;
                    return BadRequest(result);
                }

                var FieldInForm = new FieldInForm
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = body.ID,
                    FormId = body.FormId,
                    FormFieldId = body.FormFieldId,
                    IsActive = body.IsActive,
                };

                FieldInForms.Add(FieldInForm);
            }

            result = await _FieldInFormRep.EditFieldInFormsAsync(FieldInForms);
            if (result.Status)
            {
                #region AddLog

                Log log = new Log
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

        [HttpDelete("DeleteFieldInForms_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteFieldInForms_Base(List<long> ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ids);
            }

            var result = await _FieldInFormRep.RemoveFieldInFormsAsync(ids);
            if (result.Status)
            {
                #region AddLog

                Log log = new Log
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
