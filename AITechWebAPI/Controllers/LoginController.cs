using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Login;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AITechWebAPI.Controllers
{
    [Route("Login")]
    [ApiController]
    //[Authorize]
    [Produces("application/json")]

    public class LoginController : ControllerBase
    {
        ILoginMethodRep _LoginRep;
        ILogRep _logRep;

        public LoginController(ILoginMethodRep LoginRep,ILogRep logRep)
        {
           _LoginRep = LoginRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllLogins_Base")]
        public async Task<ActionResult<ListResultObject<LoginMethod>>> GetAllLogins_Base(GetLoginListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginRep.GetAllLoginMethodsAsync(requestBody.PersonId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetLoginById_Base")]
        public async Task<ActionResult<RowResultObject<LoginMethod>>> GetLoginById_Base(GetLoginRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginRep.GetLoginMethodByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }



        [HttpPost("ExistLogin_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> ExistLogin_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginRep.ExistLoginMethodAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddLogin_Base")]
        public async Task<ActionResult<BitResultObject>> AddLogin_Base(AddEditLoginRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            LoginMethod Login = new LoginMethod()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                ExpirationDate = requestBody.ExpirationDate ?? DateTime.Now.ToShamsi(),
                UserId = requestBody.UserID,
                Token = requestBody.Token,
                Method = requestBody.Method,
               // Description = requestBody.Description,
            };
            var result = await _LoginRep.AddLoginMethodAsync(Login);
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

        [HttpPut("EditLogin_Base")]
        public async Task<ActionResult<BitResultObject>> EditLogin_Base(AddEditLoginRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _LoginRep.GetLoginMethodByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            LoginMethod Login = new LoginMethod()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ExpirationDate = requestBody.ExpirationDate ?? DateTime.Now.ToShamsi(),
                UserId = requestBody.UserID,
                Token = requestBody.Token,
                Method = requestBody.Method,
                ID = requestBody.ID,
                // Description = requestBody.Description,


            };
            result = await _LoginRep.EditLoginMethodAsync(Login);
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

        [HttpDelete("DeleteLogin_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteLogin_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _LoginRep.RemoveLoginMethodAsync(requestBody.ID);
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
