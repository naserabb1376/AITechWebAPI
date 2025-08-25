using AITechDATA.CustomResponses;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.User;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("User")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]


    public class UserController : ControllerBase
    {
        IUserRep _UserRep;
        ILogRep _logRep;
        IAddressRep _addressRep;
        private readonly IMapper _mapper;


        public UserController(IUserRep UserRep,IAddressRep addressRep,ILogRep logRep,IMapper mapper)
        {
           _UserRep = UserRep;
           _logRep = logRep;
           _addressRep = addressRep;
            _mapper = mapper;   
        }

        [HttpPost("GetAllUsers_Base")]
        public async Task<ActionResult<UserListCustomResponse<UserVM>>> GetAllUsers_Base(GetUserListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.GetAllUsersAsync(requestBody.GroupId,requestBody.CourseId,requestBody.SessionAssignmentId,requestBody.SessionId,requestBody.AddressId,requestBody.RoleId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<UserListCustomResponse<UserVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("GetAllTeachers_Base")]
        public async Task<ActionResult<UserListCustomResponse<TeacherVM>>> GetAllTeachers_Base(GetUserListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.GetAllUsersAsync(requestBody.GroupId, requestBody.CourseId, requestBody.SessionAssignmentId, requestBody.SessionId, requestBody.AddressId,2, requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<UserListCustomResponse<TeacherVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetUserById_Base")]
        public async Task<ActionResult<UserRowCustomResponse<UserVM>>> GetUserById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.GetUserByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<UserRowCustomResponse<UserVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetTeacherById_Base")]
        [AllowAnonymous]
        public async Task<ActionResult<UserRowCustomResponse<TeacherVM>>> GetTeacherById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _UserRep.GetUserByIdAsync(requestBody.ID);
            if (result.Result.RoleId != 2)
            {
                result.ResultImages = new Dictionary<User, List<Image>?>();
                result.Result = new User();
            }
            if (result.Status)
            {
                var resultVM = _mapper.Map<UserRowCustomResponse<TeacherVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
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
                NationalCode = requestBody.NationalCode ?? "",
                Username = requestBody.UserName,
                AddressId = (requestBody.AdressId > 0) ? requestBody.AdressId : null,
                FullName = requestBody.FullName,
                OtherLangs = requestBody.OtherLangs ?? "",
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
        public async Task<ActionResult<BitResultObject>> EditUser_Base(EditUserRequestBody requestBody)
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
                Email = requestBody.Email ?? theRow.Result.Email,
                NationalCode = requestBody.NationalCode?? theRow.Result.NationalCode,
                Username = requestBody.UserName ?? theRow.Result.Username,
                AddressId = (requestBody.AdressId > 0) ? requestBody.AdressId : theRow.Result.AddressId,
                FullName = requestBody.FullName ?? theRow.Result.FullName,
                PasswordHash = theRow.Result.PasswordHash,
                RoleId = requestBody.RoleId ?? theRow.Result.RoleId,
                OtherLangs = requestBody.OtherLangs ?? "",

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


        [HttpPost("AddUser")]
        public async Task<ActionResult<BitResultObject>> AddUser(AddEditUserProRequestBody requestBody)
        {
            BitResultObject result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            Address address = new Address();

           if (requestBody.Address != null)
            {
                address = new Address()
                {
                    CityID = requestBody.Address.CityID,
                    AddressLocationHorizentalPoint = requestBody.Address.AddressLocationHorizentalPoint,
                    AddressLocationVerticalPoint = requestBody.Address.AddressLocationVerticalPoint,
                    AddressPostalCode = requestBody.Address.AddressPostalCode,
                    AddressStreet = requestBody.Address.AddressStreet,
                    // Description = requestBody.AddressDescription,
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    OtherLangs = requestBody.OtherLangs ?? "",


                };
                result = await _addressRep.AddAddressAsync(address);
            }

            if (result.Status)
            {

                User User = new User()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = requestBody.ID,
                    Email = requestBody.Email,
                    NationalCode = requestBody.NationalCode ?? "",
                    Username = requestBody.UserName,
                    AddressId = (address != null && address.ID > 0) ? address.ID : null,
                    FullName = requestBody.FullName,
                    PasswordHash = requestBody.Password.ToHash(),
                    RoleId = requestBody.RoleId,
                    OtherLangs = requestBody.OtherLangs ?? "",

                };
                result = await _UserRep.AddUserAsync(User);
            }

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

        [HttpPut("EditUser")]
        public async Task<ActionResult<BitResultObject>> EditUser(EditUserProRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _UserRep.GetUserByIdAsync(requestBody.ID);
            if (theRow.Result.Address== null)
            {
                theRow.Result.Address = new Address();
            }
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }


            Address address = new Address();

            if (requestBody.Address != null)
            {
                address = new Address()
                {
                    ID = theRow.Result.Address.ID,
                    CityID = requestBody.Address.CityID,
                    AddressLocationHorizentalPoint = requestBody.Address.AddressLocationHorizentalPoint,
                    AddressLocationVerticalPoint = requestBody.Address.AddressLocationVerticalPoint,
                    AddressPostalCode = requestBody.Address.AddressPostalCode,
                    AddressStreet = requestBody.Address.AddressStreet,
                    // Description = requestBody.AddressDescription,
                    CreateDate = theRow.Result.Address.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    OtherLangs = requestBody.OtherLangs ?? "",


                };
                result = await _addressRep.AddAddressAsync(address);
            }

            if (result.Status)
            {
                User User = new User()
                {
                    CreateDate = theRow.Result.CreateDate,
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ID = requestBody.ID,
                    Email = requestBody.Email,
                    NationalCode = requestBody.NationalCode ?? "",
                    Username = requestBody.UserName,
                    AddressId = (address != null && address.ID > 0) ? address.ID : null,
                    FullName = requestBody.FullName,
                    PasswordHash = theRow.Result.PasswordHash,
                    RoleId = requestBody.RoleId ?? theRow.Result.RoleId,
                    OtherLangs = requestBody.OtherLangs ?? "",
                };
                result = await _UserRep.EditUserAsync(User);
            }

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
