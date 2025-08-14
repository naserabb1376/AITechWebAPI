using AITechDATA.CustomResponses;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.City;
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
    [Route("City")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class CityController : ControllerBase
    {
        ICityRep _CityRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public CityController(ICityRep CityRep,ILogRep logRep,IMapper mapper)
        {
           _CityRep = CityRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllCities_Base")]
        public async Task<ActionResult<ListResultObject<CityVM>>> GetAllCities_Base(GetCityListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _CityRep.GetAllCitiesAsync(requestBody.ParentId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<CityVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetCityById_Base")]
        public async Task<ActionResult<RowResultObject<CityVM>>> GetCityById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _CityRep.GetCityByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<CityVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistCity_Base")]
        public async Task<ActionResult<BitResultObject>> ExistCity_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _CityRep.ExistCityAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddCity_Base")]
        public async Task<ActionResult<BitResultObject>> AddCity_Base(AddEditCiyRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            City City = new City()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                CityName = requestBody.CityName,
                CityParentID = requestBody.ParentId,
                //Description = requestBody.Description,
                UpdateDate = DateTime.Now.ToShamsi(),
                DefaultCity = requestBody.DefaultCity,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            var result = await _CityRep.AddCityAsync(City);
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

        [HttpPut("EditCity_Base")]
        public async Task<ActionResult<BitResultObject>> EditCity_Base(AddEditCiyRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var theRow = await _CityRep.GetCityByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            City City = new City()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                CityName = requestBody.CityName,
                CityParentID = requestBody.ParentId,
                //Description = requestBody.Description,
                OtherLangs = requestBody.OtherLangs ?? "",

            };
            result = await _CityRep.EditCityAsync(City);
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

        [HttpDelete("DeleteCity_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteCity_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _CityRep.RemoveCityAsync(requestBody.ID);
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
