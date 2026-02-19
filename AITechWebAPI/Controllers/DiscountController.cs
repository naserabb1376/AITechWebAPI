using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.Discount;
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
    [Route("Discount")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class DiscountController : ControllerBase
    {
        private IDiscountRep _DiscountRep;
        private ILogRep _logRep;
        private readonly IMapper _mapper;


        public DiscountController(IDiscountRep DiscountRep, ILogRep logRep,IMapper mapper)
        {
            _DiscountRep = DiscountRep;
            _logRep = logRep;
            _mapper = mapper;
        }

        [HttpPost("GetAllDiscounts_Base")]
        public async Task<ActionResult<ListResultObject<DiscountVM>>> GetAllDiscounts_Base(GetDiscountListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountRep.GetAllDiscountsAsync(requestBody.EntityName,requestBody.ForeignKeyId,requestBody.CreatorId,requestBody.PageIndex, requestBody.PageSize, requestBody.SearchText, requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<DiscountVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetDiscountById_Base")]
        public async Task<ActionResult<RowResultObject<DiscountVM>>> GetDiscountById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountRep.GetDiscountByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<DiscountVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }


        [HttpPost("ExistDiscount_Base")]
        public async Task<ActionResult<BitResultObject>> ExistDiscount_Base(ExistDiscountRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountRep.ExistDiscountAsync(requestBody.ExistType,requestBody.KeyValue);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddDiscount_Base")]
        public async Task<ActionResult<RowResultObject<Discount>>> AddDiscount_Base(AddEditDiscountRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            Discount Discount = new Discount()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                OtherLangs = requestBody.OtherLangs,
                Description = requestBody.Description,
                IsActive = requestBody.IsActive,
                CreatorId = requestBody.CreatorId ?? User.GetCurrentUserId(),
                EntityName = requestBody.EntityName,
                ForeignKeyId = requestBody.ForeignKeyId,
                CodeRequired = requestBody.CodeRequired,
                DiscountPercent = requestBody.DiscountPercent,
                ExpireDate = requestBody.ExpireDate.StringToDate().Value,
                DiscountCode = requestBody.DiscountCode.GenerateDiscountCode(),

            };
            var result = await _DiscountRep.AddDiscountAsync(Discount);
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

        [HttpPut("EditDiscount_Base")]
        public async Task<ActionResult<RowResultObject<Discount>>> EditDiscount_Base(AddEditDiscountRequestBody requestBody)
        {
            var result = new RowResultObject<Discount>();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _DiscountRep.GetDiscountByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            Discount Discount = new Discount()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                OtherLangs = requestBody.OtherLangs,
                Description = requestBody.Description,
                IsActive = requestBody.IsActive,
                CreatorId = requestBody.CreatorId ?? theRow.Result.CreatorId,
                EntityName = requestBody.EntityName,
                ForeignKeyId = requestBody.ForeignKeyId,
                CodeRequired = requestBody.CodeRequired,
                DiscountPercent = requestBody.DiscountPercent,
                ExpireDate = requestBody.ExpireDate.StringToDate().Value,
                DiscountCode = requestBody.DiscountCode.GenerateDiscountCode(), 
            };
            result = await _DiscountRep.EditDiscountAsync(Discount);
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

        [HttpDelete("DeleteDiscount_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteDiscount_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _DiscountRep.RemoveDiscountAsync(requestBody.ID);
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