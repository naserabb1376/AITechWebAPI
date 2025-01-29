using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PaymentHistory;
using AITechWebAPI.Models.Public;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AiTech.Domains;

namespace AITechWebAPI.Controllers
{
    [Route("PaymentHistory")]
    [ApiController]
    //[Authorize]
    [Produces("application/json")]

    public class PaymentHistoryController : ControllerBase
    {
        IPaymentHistoryRep _PaymentHistoryRep;
        ILogRep _logRep;

        public PaymentHistoryController(IPaymentHistoryRep PaymentHistoryRep,ILogRep logRep)
        {
           _PaymentHistoryRep = PaymentHistoryRep;
           _logRep = logRep;
        }

        [HttpPost("GetAllPaymentHistories_Base")]
        public async Task<ActionResult<ListResultObject<PaymentHistory>>> GetAllPaymentHistories_Base(GetPaymentHistoryListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.GetAllPaymentHistoriesAsync(requestBody.GroupId,requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("GetPaymentHistoryById_Base")]
        public async Task<ActionResult<RowResultObject<PaymentHistory>>> GetPaymentHistoryById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(requestBody.ID);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistPaymentHistory_Base")]
        public async Task<ActionResult<BitResultObject>> ExistPaymentHistory_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.ExistPaymentHistoryAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddPaymentHistory_Base")]
        public async Task<ActionResult<BitResultObject>> AddPaymentHistory_Base(AddEditPaymentHistoryRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            PaymentHistory PaymentHistory = new PaymentHistory()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                GroupId = requestBody.GroupID,
                Amount = requestBody.Amount,
                UserId = requestBody.UserID,
                PaymentDate = requestBody.PaymentDate.StringToDate(),
              //  Description = requestBody.Description,
            };
            var result = await _PaymentHistoryRep.AddPaymentHistoryAsync(PaymentHistory);
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

        [HttpPut("EditPaymentHistory_Base")]
        public async Task<ActionResult<BitResultObject>> EditPaymentHistory_Base(AddEditPaymentHistoryRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            PaymentHistory PaymentHistory = new PaymentHistory()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                GroupId = requestBody.GroupID,
                Amount = requestBody.Amount,
                UserId = requestBody.UserID,
                PaymentDate = requestBody.PaymentDate.StringToDate(),
               // Description = requestBody.Description,
            };
            result = await _PaymentHistoryRep.EditPaymentHistoryAsync(PaymentHistory);
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

        [HttpDelete("DeletePaymentHistory_Base")]
        public async Task<ActionResult<BitResultObject>> DeletePaymentHistory_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.RemovePaymentHistoryAsync(requestBody.ID);
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
