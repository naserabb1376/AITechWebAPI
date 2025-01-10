using Domain;
using Domains;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Servisces;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NobatPlusAPI.Controllers
{
    [Route("User")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class UserController : ControllerBase
    {
        IUserRep _UserRep;
        ILogRep _logRep;
        IAddressRep _addressRep;

        public UserController(IUserRep UserRep,IAddressRep addressRep,ILogRep logRep)
        {
           _UserRep = UserRep;
           _logRep = logRep;
           _addressRep = addressRep;
        }

        [HttpPost("GetAllUsers_Base")]
        public async Task<ActionResult<ListResultObject<User>>> GetAllUsers_Base(GetUserListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.GetAllUsersAsync(requestBody.CityId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetUserById_Base")]
        public async Task<ActionResult<RowResultObject<User>>> GetUserById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.GetUserByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistUser_Base")]
        public async Task<ActionResult<BitResultObject>> ExistUser_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.ExistUserAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }




        [HttpPost("AddUser_Base")]
        public async Task<ActionResult<BitResultObject>> AddUser_Base(AddEditUserRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            User User = new User()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                AddressID = requestBody.AdressId,
                DateOfBirth = requestBody.DateOfBirth?.StringToDate(),
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                Email = requestBody.Email,
                NaCode = requestBody.NaCode,
                PhoneNumber = requestBody.PhoneNumber,
                Description = requestBody.Description,
            };
            var result = await _UserRep.AddUserAsync(User);
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

        [HttpPut("EditUser_Base")]
        public async Task<ActionResult<BitResultObject>> EditUser_Base(AddEditUserRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _UserRep.GetUserByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            User User = new User()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                AddressID = requestBody.AdressId,
                DateOfBirth = requestBody.DateOfBirth?.StringToDate(),
                FirstName = requestBody.FirstName,
                LastName = requestBody.LastName,
                Email = requestBody.Email,
                NaCode = requestBody.NaCode,
                PhoneNumber = requestBody.PhoneNumber,
                Description = requestBody.Description,
            };
            result = await _UserRep.EditUserAsync(User);
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

        [HttpDelete("DeleteUser_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteUser_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.RemoveUserAsync(requestBody.ID);
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
