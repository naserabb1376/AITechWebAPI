using AiTech.Domains;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PaymentHistory;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Parbad;
using Parbad.Abstraction;
using Parbad.Gateway.ZarinPal;
using Parbad.InvoiceBuilder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechWebAPI.Controllers
{
    [Route("PaymentHistory")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class PaymentHistoryController : ControllerBase
    {
        private readonly IOnlinePayment _onlinePayment;
        IPaymentHistoryRep _PaymentHistoryRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PaymentHistoryController(IPaymentHistoryRep PaymentHistoryRep,ILogRep logRep,IMapper mapper, IOnlinePayment onlinePayment)
        {
            _onlinePayment = onlinePayment;
            _PaymentHistoryRep = PaymentHistoryRep;
           _logRep = logRep;
            _mapper = mapper;
        }




        [HttpPost("GetAllPaymentHistories_Base")]
        public async Task<ActionResult<ListResultObject<PaymentHistoryVM>>> GetAllPaymentHistories_Base(GetPaymentHistoryListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.GetAllPaymentHistoriesAsync(requestBody.GroupId,requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<PaymentHistoryVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetPaymentHistoryById_Base")]
        public async Task<ActionResult<RowResultObject<PaymentHistoryVM>>> GetPaymentHistoryById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<PaymentHistoryVM>>(result);
                return Ok(resultVM);
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
                PaymentDate =  string.IsNullOrEmpty(requestBody.PaymentDate) ? DateTime.Now.ToShamsi() : requestBody.PaymentDate.StringToDate().Value,
                PaymentStatus = false,
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


                #region PaymentGateway

                var zarInvoice = new ZarinPalInvoice(description: $"{result.ID} - {PaymentHistory.PaymentDate}");
                var callbackUrl = Url.Action("VerifyPayment", "PaymentHistory", new { payId = result.ID }, Request.Scheme);

                var invoice = await _onlinePayment.RequestAsync(invoice =>
                {
                    invoice.SetCallbackUrl(callbackUrl)
                           .SetAmount(requestBody.Amount)
                            .SetZarinPalData(zarInvoice)
                            .UseZarinPal();

                    invoice.UseAutoIncrementTrackingNumber();

                });

                if (invoice.IsSucceed)
                {
                    result.ErrorMessage = invoice.GatewayTransporter.Descriptor.Url;
                }

                else
                {
                    result.ErrorMessage = invoice.Message;
                }

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
                PaymentDate = string.IsNullOrEmpty(requestBody.PaymentDate) ? DateTime.Now.ToShamsi() : requestBody.PaymentDate.StringToDate().Value,
                PaymentStatus = requestBody.PaymentStatus,
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

        [HttpGet("VerifyPayment")]
        [AllowAnonymous]
        public async Task<ActionResult<BitResultObject>> VerifyPayment(long payId, string? paymentToken = "", string? Authority ="",string? Status = "")
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var paymentHistory = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(payId);

                var invoice = await _onlinePayment.FetchAsync();

                // Check if the invoice is new or it's already processed before.
                if (invoice.Status != PaymentFetchResultStatus.ReadyForVerifying)
                {
                    // You can also see if the invoice is already verified before.
                    paymentHistory.Result.PaymentStatus = false;
                }

                var verifyResult = await _onlinePayment.VerifyAsync(invoice);

                var originalResult = verifyResult.GetZarinPalOriginalVerificationResult();

                // Note: Save the verifyResult.TransactionCode in your database.

                if (verifyResult.Status == PaymentVerifyResultStatus.Succeed)
                {
                    paymentHistory.Result.PaymentStatus = true;
                }

                else
                {
                    paymentHistory.Result.PaymentStatus = false;
                }

                var saveResult = await _PaymentHistoryRep.EditPaymentHistoryAsync(paymentHistory.Result);

                if (saveResult.Status)
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

                if (paymentHistory.Result.PaymentStatus)
                {
                    result.Status = true;
                    result.ErrorMessage = $"پرداخت موفقیت آمیز بود";
                    result.ID = paymentHistory.Result.ID;
                    return Ok(result);
                }

                else
                {
                    result.Status = true;
                    result.ErrorMessage = $"پرداخت ناموفق بود";
                    result.ID = paymentHistory.Result.ID;
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
          
        }
    }
}
