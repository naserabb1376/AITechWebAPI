using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PreRegistration;
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
    [Route("PreRegistration")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin, (int)BaseRole.ContentAdmin })]

    public class PreRegistrationController : ControllerBase
    {
        IPreRegistrationRep _PreRegistrationRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PreRegistrationController(IPreRegistrationRep PreRegistrationRep,ILogRep logRep,IMapper mapper)
        {
           _PreRegistrationRep = PreRegistrationRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllPreRegistrations_Base")]
        public async Task<ActionResult<ListResultObject<PreRegistrationVM>>> GetAllPreRegistrations_Base(GetPreRegistrationListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.GetAllPreRegistrationsAsync(requestBody.ForeignKeyId,requestBody.EntityType,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<PreRegistrationVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetPreRegistrationById_Base")]
        public async Task<ActionResult<RowResultObject<PreRegistrationVM>>> GetPreRegistrationById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.GetPreRegistrationByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<PreRegistrationVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistPreRegistration_Base")]
        public async Task<ActionResult<BitResultObject>> ExistPreRegistration_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.ExistPreRegistrationAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddPreRegistration_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> AddPreRegistration_Base(AddEditPreRegistrationRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            PreRegistration PreRegistration = new PreRegistration()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email,
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,

                EducationalClass = requestBody.EducationalClass,
                SchoolName = requestBody.SchoolName,
                FavoriteField = requestBody.FavoriteField,
                RecognitionLevel = requestBody.RecognitionLevel,
                ProgrammingSkillLevel = requestBody.ProgrammingSkillLevel,

                ForeignKeyId = requestBody.ForeignKeyId,
                EntityType = requestBody.EntityType,
                RegistrationDate = requestBody.RegistrationDate.StringToDate().Value,
                OtherLangs = requestBody.OtherLangs ?? "",
                // Description = requestBody.Description,
            };
            var result = await _PreRegistrationRep.AddPreRegistrationAsync(PreRegistration);
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

        [HttpPut("EditPreRegistration_Base")]
        public async Task<ActionResult<BitResultObject>> EditPreRegistration_Base(AddEditPreRegistrationRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _PreRegistrationRep.GetPreRegistrationByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            PreRegistration PreRegistration = new PreRegistration()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                Email = requestBody.Email,
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                PhoneNumber = requestBody.PhoneNumber,

                EducationalClass = requestBody.EducationalClass,
                SchoolName = requestBody.SchoolName,
                FavoriteField = requestBody.FavoriteField,
                RecognitionLevel = requestBody.RecognitionLevel,
                ProgrammingSkillLevel = requestBody.ProgrammingSkillLevel,

                ForeignKeyId = requestBody.ForeignKeyId,
                EntityType = requestBody.EntityType,
                RegistrationDate = requestBody.RegistrationDate.StringToDate().Value,
                ID = requestBody.ID,
                OtherLangs = requestBody.OtherLangs ?? "",
                // Description = requestBody.Description,


            };
            result = await _PreRegistrationRep.EditPreRegistrationAsync(PreRegistration);
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

        [HttpDelete("DeletePreRegistration_Base")]
        public async Task<ActionResult<BitResultObject>> DeletePreRegistration_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.RemovePreRegistrationAsync(requestBody.ID);
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

        [HttpPost("GetRegistrationTypes_Base")]
        public async Task<ActionResult<ListResultObject<PreRegistrationVM>>> GetRegistrationTypes_Base(GetRegistrationTypesListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PreRegistrationRep.GetRegistrationTypesAsync(requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
