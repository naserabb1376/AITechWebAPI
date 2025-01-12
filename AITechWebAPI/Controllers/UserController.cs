using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AITechWebAPI.Models.User;

namespace AITechWebAPI.Controllers
{
    [Route("User")]
    [ApiController]
    //[Authorize]
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
            var result = await _UserRep.GetAllUsersAsync(requestBody.AddressId,requestBody.RoleId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
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
        public async Task<ActionResult<BitResultObject>> ExistUser_Base(ExistUserRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.ExistUserAsync(requestBody.FieldValue,requestBody.FieldName);
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
                ID = requestBody.ID,
                Email = requestBody.Email,
                Username = requestBody.Username,
                AddressId = requestBody.AdressId,
                FullName = requestBody.FullName,
                PasswordHash = requestBody.Password.ToHash(),
                RoleId = requestBody.RoleId,

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
                Email = requestBody.Email,
                Username = requestBody.Username,
                AddressId = requestBody.AdressId,
                FullName = requestBody.FullName,
                PasswordHash = requestBody.Password.ToHash(),
                RoleId = requestBody.RoleId,
                
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


        //[HttpPost("AddUser")]
        //public async Task<ActionResult<BitResultObject>> AddUser(AddEditUserProRequestBody requestBody)
        //{
        //    BitResultObject result = new BitResultObject();
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(requestBody);
        //    }

        //    Address address = new Address()
        //    {
        //        CityID = requestBody.CityID,
        //        AddressLocationHorizentalPoint = requestBody.AddressLocationHorizentalPoint,
        //        AddressLocationVerticalPoint = requestBody.AddressLocationVerticalPoint,
        //        AddressPostalCode = requestBody.AddressPostalCode,
        //        AddressStreet = requestBody.AddressStreet,
        //       // Description = requestBody.AddressDescription,
        //        CreateDate = DateTime.Now.ToShamsi(),
        //        UpdateDate = DateTime.Now.ToShamsi(),

        //    };

        //    if (result.Status)
        //    {
        //        result = await _addressRep.AddAddressAsync(address);

        //        User User = new User()
        //        {
        //            CreateDate = DateTime.Now.ToShamsi(),
        //            UpdateDate = DateTime.Now.ToShamsi(),
        //            AddressID = address.ID,
        //            DateOfBirth = requestBody.DateOfBirth?.StringToDate(),
        //            FirstName = requestBody.FirstName,
        //            LastName = requestBody.LastName,
        //            Email = requestBody.Email,
        //            NaCode = requestBody.NaCode,
        //            PhoneNumber = requestBody.PhoneNumber,
        //            Description = requestBody.Description,
        //        };
        //        result = await _UserRep.AddUserAsync(User);
        //    }

        //    if (result.Status)
        //    {
        //        #region AddLog

        //        Log log = new Log()
        //        {
        //            CreateDate = DateTime.Now.ToShamsi(),
        //            UpdateDate = DateTime.Now.ToShamsi(),
        //            LogTime = DateTime.Now.ToShamsi(),
        //            ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

        //        };
        //        await _logRep.AddLogAsync(log);

        //        #endregion


        //        return Ok(result);
        //    }

        //    return BadRequest(result);
        //}

        //[HttpPut("EditUser")]
        //public async Task<ActionResult<BitResultObject>> EditUser(AddEditUserProRequestBody requestBody)
        //{
        //    var result = new BitResultObject();
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(requestBody);
        //    }
        //    var theRow = await _UserRep.GetUserByIdAsync(requestBody.ID);
        //    if (!theRow.Status)
        //    {
        //        result.Status = theRow.Status;
        //        result.ErrorMessage = theRow.ErrorMessage;
        //    }

        //    Address address = new Address()
        //    {
        //        CityID = requestBody.CityID,
        //        ID = theRow.Result.AddressID,
        //        AddressLocationHorizentalPoint = requestBody.AddressLocationHorizentalPoint,
        //        AddressLocationVerticalPoint = requestBody.AddressLocationVerticalPoint,
        //        AddressPostalCode = requestBody.AddressPostalCode,
        //        AddressStreet = requestBody.AddressStreet,
        //        Description = requestBody.AddressDescription,
        //        CreateDate = theRow.Result.Address.CreateDate,
        //        UpdateDate = DateTime.Now.ToShamsi(),

        //    };

        //    if (result.Status)
        //    {
        //        result = await _addressRep.EditAddressAsync(address);

        //        User User = new User()
        //        {
        //            CreateDate = theRow.Result.CreateDate,
        //            UpdateDate = DateTime.Now.ToShamsi(),
        //            ID = requestBody.ID,
        //            AddressID = address.ID,
        //            DateOfBirth = requestBody.DateOfBirth?.StringToDate(),
        //            FirstName = requestBody.FirstName,
        //            LastName = requestBody.LastName,
        //            Email = requestBody.Email,
        //            NaCode = requestBody.NaCode,
        //            PhoneNumber = requestBody.PhoneNumber,
        //            Description = requestBody.Description,
        //        };
        //        result = await _UserRep.EditUserAsync(User);
        //    }

        //    if (result.Status)
        //    {

        //        #region AddLog

        //        Log log = new Log()
        //        {
        //            CreateDate = DateTime.Now.ToShamsi(),
        //            UpdateDate = DateTime.Now.ToShamsi(),
        //            LogTime = DateTime.Now.ToShamsi(),
        //            ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

        //        };
        //        await _logRep.AddLogAsync(log);

        //        #endregion

        //        return Ok(result);
        //    }
        //    return BadRequest(result);
        //}


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
