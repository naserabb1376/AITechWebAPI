using AiTech.Domains;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PaymentInstallment;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Tools;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    [Route("PaymentInstallment")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class PaymentInstallmentController : ControllerBase
    {
        IPaymentInstallmentRep _PaymentInstallmentRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PaymentInstallmentController(IPaymentInstallmentRep paymentInstallmentRep,ILogRep logRep,IMapper mapper)
        {
            _PaymentInstallmentRep = paymentInstallmentRep;
           _logRep = logRep;
            _mapper = mapper;
        }




        [HttpPost("GetAllPaymentInstallments_Base")]
        public async Task<ActionResult<ListResultObject<PaymentInstallmentVM>>> GetAllPaymentInstallments_Base(GetPaymentInstallmentListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentInstallmentRep.GetAllPaymentInstallmentsAsync(requestBody.PaymentHistoryId,requestBody.UserId,requestBody.PayState,requestBody.AlowState,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<PaymentInstallmentVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetPaymentInstallmentById_Base")]
        public async Task<ActionResult<RowResultObject<PaymentInstallmentVM>>> GetPaymentInstallmentById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentInstallmentRep.GetPaymentInstallmentByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<PaymentInstallmentVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistPaymentInstallment_Base")]
        public async Task<ActionResult<BitResultObject>> ExistPaymentInstallment_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentInstallmentRep.ExistPaymentInstallmentAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddPaymentInstallment_Base")]
        public async Task<ActionResult<BitResultObject>> AddPaymentInstallment_Base(AddEditPaymentInstallmentRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            PaymentInstallment PaymentInstallment = new PaymentInstallment()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                InstallmentNumber = requestBody.InstallmentNumber,
                IsActive = requestBody.IsActive,
                Amount = requestBody.Amount,
                PaymentHistoryId = requestBody.PaymentHistoryID,
                PaidDate =  string.IsNullOrEmpty(requestBody.PaidDate) ? DateTime.Now.ToShamsi() : requestBody.PaidDate.StringToDate().Value,
                DueDate =  string.IsNullOrEmpty(requestBody.DueDate) ? DateTime.Now.ToShamsi() : requestBody.DueDate.StringToDate().Value,
                IsPaid = requestBody.IsPaid,
                PayAllowed = requestBody.PayAllowed,
            };
            var result = await _PaymentInstallmentRep.AddPaymentInstallmentAsync(PaymentInstallment);
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

        [HttpPut("EditPaymentInstallment_Base")]
        public async Task<ActionResult<BitResultObject>> EditPaymentInstallment_Base(AddEditPaymentInstallmentRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var theRow = await _PaymentInstallmentRep.GetPaymentInstallmentByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            PaymentInstallment PaymentInstallment = new PaymentInstallment()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                InstallmentNumber = requestBody.InstallmentNumber,
                IsActive = requestBody.IsActive,
                Amount = requestBody.Amount,
                PaymentHistoryId = requestBody.PaymentHistoryID,
                PaidDate = string.IsNullOrEmpty(requestBody.PaidDate) ? DateTime.Now.ToShamsi() : requestBody.PaidDate.StringToDate().Value,
                DueDate = string.IsNullOrEmpty(requestBody.DueDate) ? DateTime.Now.ToShamsi() : requestBody.DueDate.StringToDate().Value,
                IsPaid = requestBody.IsPaid,
                PayAllowed = requestBody.PayAllowed,
            };
            result = await _PaymentInstallmentRep.EditPaymentInstallmentAsync(PaymentInstallment);
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

        [HttpDelete("DeletePaymentInstallment_Base")]
        public async Task<ActionResult<BitResultObject>> DeletePaymentInstallment_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentInstallmentRep.RemovePaymentInstallmentAsync(requestBody.ID);
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
