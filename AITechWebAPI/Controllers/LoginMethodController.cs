using AITechDATA.CustomResponses;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.LoginMethod;
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
    [Route("LoginMethod")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] {(int)BaseRole.GeneralAdmin })]

    public class LoginMethodController : ControllerBase
    {
        ILoginMethodRep _LoginMethodRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public LoginMethodController(ILoginMethodRep LoginMethodRep,ILogRep logRep,IMapper mapper)
        {
           _LoginMethodRep = LoginMethodRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllLoginMethods_Base")]
        public async Task<ActionResult<ListResultObject<LoginMethodVM>>> GetAllLoginMethods_Base(GetLoginMethodListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginMethodRep.GetAllLoginMethodsAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<LoginMethodVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetLoginMethodById_Base")]
        public async Task<ActionResult<RowResultObject<LoginMethodVM>>> GetLoginMethodById_Base(GetLoginMethodRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginMethodRep.GetLoginMethodByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<LoginMethodVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistLoginMethod_Base")]
        public async Task<ActionResult<BitResultObject>> ExistLoginMethod_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginMethodRep.ExistLoginMethodAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddLoginMethod_Base")]
        public async Task<ActionResult<BitResultObject>> AddLoginMethod_Base(AddEditLoginMethodRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            LoginMethod LoginMethod = new LoginMethod()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                ExpirationDate = requestBody.ExpirationDate.StringToDate(),
                UserId = requestBody.UserID,
                Token = requestBody.Token,
                Method = requestBody.Method,
               // Description = requestBody.Description,
            };
            var result = await _LoginMethodRep.AddLoginMethodAsync(LoginMethod);
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

        [HttpPut("EditLoginMethod_Base")]
        public async Task<ActionResult<BitResultObject>> EditLoginMethod_Base(AddEditLoginMethodRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _LoginMethodRep.GetLoginMethodByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            LoginMethod LoginMethod = new LoginMethod()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ExpirationDate = requestBody.ExpirationDate.StringToDate(),
                UserId = requestBody.UserID,
                Token = requestBody.Token,
                Method = requestBody.Method,
                ID = requestBody.ID,
                // Description = requestBody.Description,


            };
            result = await _LoginMethodRep.EditLoginMethodAsync(LoginMethod);
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

        [HttpDelete("DeleteLoginMethod_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteLoginMethod_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginMethodRep.RemoveLoginMethodAsync(requestBody.ID);
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
