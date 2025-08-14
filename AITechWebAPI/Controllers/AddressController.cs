using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Address;
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
    [Route("Address")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class AddressController : ControllerBase
    {
        IAddressRep _AddressRep;
        IUserRep _userRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public AddressController(IAddressRep AddressRep,IUserRep userRep,ILogRep logRep,IMapper mapper)
        {
           _AddressRep = AddressRep;
            _userRep = userRep;
           _logRep = logRep;
           _mapper = mapper;
        }

        [HttpPost("GetAllAddresses_Base")]
        public async Task<ActionResult<ListResultObject<AddressVM>>> GetAllAddresses_Base(GetAddressListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AddressRep.GetAllAddressesAsync(requestBody.CityId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<AddressVM>>(result);
                return Ok(resultVM);

            }
            return BadRequest(result);
        }

        [HttpPost("GetAddressById_Base")]
        public async Task<ActionResult<RowResultObject<AddressVM>>> GetAddressById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AddressRep.GetAddressByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<AddressVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistAddress_Base")]
        public async Task<ActionResult<BitResultObject>> ExistAddress_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AddressRep.ExistAddressAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddAddress_Base")]
        public async Task<ActionResult<BitResultObject>> AddAddress_Base(AddEditSelfAddressRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theUser = await _userRep.GetUserByIdAsync(requestBody.UserID);
            if (theUser.Result == null)
            {
                theUser.ErrorMessage = $"این کاربر در سیستم وجود ندارد";
                return BadRequest(theUser);
            }

            Address Address = new Address()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                CityID = requestBody.CityID,
                AddressLocationHorizentalPoint = requestBody.AddressLocationHorizentalPoint,
                AddressLocationVerticalPoint = requestBody.AddressLocationVerticalPoint,
                AddressPostalCode = requestBody.AddressPostalCode,
                AddressStreet = requestBody.AddressStreet,
                //Description = requestBody.AddressDescription,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            var result = await _AddressRep.AddAddressAsync(Address);
            if (result.Status)
            {
                theUser.Result.AddressId = Address.ID;

                result = await _userRep.EditUserAsync(theUser.Result);

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

                }
                    return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("EditAddress_Base")]
        public async Task<ActionResult<BitResultObject>> EditAddress_Base(AddEditAddressRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var theRow = await _AddressRep.GetAddressByIdAsync(requestBody.ID);

            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Address Address = new Address()
            {
                UpdateDate = DateTime.Now.ToShamsi(),
                 ID = requestBody.ID,
                 CreateDate = theRow.Result.CreateDate,
                CityID = requestBody.CityID,
                AddressLocationHorizentalPoint = requestBody.AddressLocationHorizentalPoint,
                AddressLocationVerticalPoint = requestBody.AddressLocationVerticalPoint,
                AddressPostalCode = requestBody.AddressPostalCode,
                AddressStreet = requestBody.AddressStreet,
                //Description = requestBody.AddressDescription,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
             result = await _AddressRep.EditAddressAsync(Address);
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

        [HttpDelete("DeleteAddress_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteAddress_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _AddressRep.RemoveAddressAsync(requestBody.ID);
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
