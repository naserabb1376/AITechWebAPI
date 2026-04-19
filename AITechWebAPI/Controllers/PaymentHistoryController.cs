using AiTech.Domains;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models;
using AITechWebAPI.Models.PaymentHistory;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Tools;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("PaymentHistory")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class PaymentHistoryController : ControllerBase
    {
        private readonly IOnlinePayment _onlinePayment;
        IPaymentHistoryRep _PaymentHistoryRep;
        IGroupRep _GroupRep;
        IEventRep _EventRep;
        IUserRep _UserRep;
        IUserGroupRep _UserGroupRep;
        IPreRegistrationRep _PreRegistrationRep;
        IDiscountRep _discountRep;
        ISettingRep _settingRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PaymentHistoryController(IPaymentHistoryRep PaymentHistoryRep,IGroupRep groupRep,IEventRep eventRep,IUserRep userRep,IUserGroupRep userGroupRep,IPreRegistrationRep preRegistrationRep,IDiscountRep discountRep,ISettingRep settingRep,ILogRep logRep,IMapper mapper, IOnlinePayment onlinePayment)
        {
            _onlinePayment = onlinePayment;
            _PaymentHistoryRep = PaymentHistoryRep;
            _GroupRep = groupRep;
            _EventRep = eventRep;
            _UserRep = userRep;
            _UserGroupRep = userGroupRep;
            _PreRegistrationRep = preRegistrationRep;
            _discountRep = discountRep;
            _settingRep = settingRep;
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
            var result = await _PaymentHistoryRep.GetAllPaymentHistoriesAsync(requestBody.ForeignKeyId,requestBody.EntityType,requestBody.UserId,requestBody.DiscountId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
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

            dynamic targetObj = requestBody.EntityType.ToLower().Contains("event") ? await _EventRep.GetEventByIdAsync(requestBody.ForeignKeyId) :
await _GroupRep.GetGroupByIdAsync(requestBody.ForeignKeyId);

            string targetObjName = requestBody.EntityType.ToLower().Contains("event") ? targetObj.Result.Title : targetObj.Result.Name;

            PaymentHistory PaymentHistory = new PaymentHistory()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                ForeignKeyId = requestBody.ForeignKeyId,
                EntityType = requestBody.EntityType,
                TargetObjName = targetObjName,
                IsActive = requestBody.IsActive,
                Amount = requestBody.Amount,
                UserId = requestBody.UserID,
                PaymentDate =  string.IsNullOrEmpty(requestBody.PaymentDate) ? DateTime.Now.ToShamsi() : requestBody.PaymentDate.StringToDate().Value,
                PaymentStatus = false,
                DiscountId = requestBody.DiscountId,
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

            dynamic targetObj = requestBody.EntityType.ToLower().Contains("event") ? await _EventRep.GetEventByIdAsync(requestBody.ForeignKeyId) :
await _GroupRep.GetGroupByIdAsync(requestBody.ForeignKeyId);

            string targetObjName = requestBody.EntityType.ToLower().Contains("event") ? targetObj.Result.Title : targetObj.Result.Name;


            PaymentHistory PaymentHistory = new PaymentHistory()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                ForeignKeyId = requestBody.ForeignKeyId,
                EntityType = requestBody.EntityType,
                TargetObjName = targetObjName,
                IsActive = requestBody.IsActive,
                Amount = requestBody.Amount,
                UserId = requestBody.UserID,
                PaymentDate = string.IsNullOrEmpty(requestBody.PaymentDate) ? DateTime.Now.ToShamsi() : requestBody.PaymentDate.StringToDate().Value,
                PaymentStatus = requestBody.PaymentStatus,
                DiscountId = requestBody.DiscountId,
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

        [HttpPost("RequestPayment")]
        public async Task<ActionResult<RowResultObject<RequestPaymentResultBody>>> RequestPayment(RequestPaymentRequestBody requestBody)
        {
            RowResultObject<RequestPaymentResultBody> result = new RowResultObject<RequestPaymentResultBody>();
            result.Result = new RequestPaymentResultBody();
            decimal rowAmount = 0,discountedrowAmount = 0;
            string targetObjName="",groupType="";
            BitResultObject addResult;
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var UserId = User.GetCurrentUserId();

            switch (requestBody.EntityType.ToLower())
            {
                default:
                case "group":
                    {
                         var theRow = await _GroupRep.GetGroupByIdAsync(requestBody.ForeignKeyId);

                        if (theRow.Result == null)
                        {
                            result.Result = null;
                            result.Status = false ;
                            result.ErrorMessage = "درخواست نامعتبر است" ;
                            return BadRequest(result);
                        }
                        if (theRow.Result.GroupCapacity <= 0)
                        {
                            result.Result = null;
                            result.Status = false;
                            result.ErrorMessage = "ظرفیت ثبت نام این گروه تکمیل شده است";
                            return BadRequest(result);
                        }

                        rowAmount = theRow.Result.Fee;
                        targetObjName = theRow.Result.Name;
                        groupType = theRow.Result.GroupType.ToLower();
                        discountedrowAmount = theRow.Result.DiscountedFee;

                    }
                    break;
                case "event":
                    {
                         var theRow = await _EventRep.GetEventByIdAsync(requestBody.ForeignKeyId);

                        if (theRow.Result == null)
                        {
                            result.Result = null;
                            result.Status = false;
                            result.ErrorMessage = "درخواست نامعتبر است";
                            return BadRequest(result);
                        }

                        rowAmount = theRow.Result.Fee ?? 0;
                        targetObjName = theRow.Result.Title;
                        discountedrowAmount = theRow.Result.DiscountedFee ?? 0;


                    }
                    break;
            }

            if (discountedrowAmount == rowAmount && requestBody.DiscountId > 0)
            {
                var discount = await _discountRep.GetDiscountByIdAsync(requestBody.DiscountId.Value);
                if (discount.Result.IsActive && discount.Result.EntityName.ToLower() == requestBody.EntityType.ToLower() && discount.Result.ForeignKeyId == requestBody.ForeignKeyId && DateTime.Now.ToShamsi() >= discount.Result.ExpireDate)
                {
                    discountedrowAmount =  rowAmount - (rowAmount * discount.Result.DiscountPercent / 100m);
                }
            }

            rowAmount = discountedrowAmount;

            if (rowAmount > 0)
            {
                PaymentHistory PaymentHistory = new PaymentHistory()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    ForeignKeyId = requestBody.ForeignKeyId,
                    EntityType = requestBody.EntityType,
                    TargetObjName = targetObjName,
                    Amount = rowAmount,
                    UserId = UserId,
                    PaymentDate = DateTime.Now.ToShamsi(),
                    PaymentStatus = false,

                    //  Description = requestBody.Description,
                };
                var Addresult = await _PaymentHistoryRep.AddPaymentHistoryAsync(PaymentHistory);
                if (Addresult.Status)
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

                    var zarInvoice = new ZarinPalInvoice(description: $"{Addresult.ID} - {PaymentHistory.PaymentDate}");
                    // var callbackUrl = Url.Action("VerifyPayment", "PaymentHistory", new { payId = Addresult.ID }, Request.Scheme);
                    var callbackUrl = $"https://aitechac.com/payment/verify?PayId={Addresult.ID}&UserId={UserId}&EntityType={requestBody.EntityType}&ForeignKeyId={requestBody.ForeignKeyId}";

                    var invoice = await _onlinePayment.RequestAsync(invoice =>
                    {
                        invoice.SetCallbackUrl(callbackUrl)
                               .SetAmount(rowAmount)
                                .SetZarinPalData(zarInvoice)
                                .UseZarinPal();

                        invoice.UseAutoIncrementTrackingNumber();

                    });

                    if (invoice.IsSucceed)
                    {
                        result.Result.PayGatewayUrl = invoice.GatewayTransporter.Descriptor.Url;
                        result.ErrorMessage = "";
                    }



                    else
                    {
                        result.ErrorMessage = invoice.Message;
                        result.Result.PayGatewayUrl = "";
                    }

                    #endregion



                    return Ok(result);
                }
            }

            else
            {
                var userRow = await _UserRep.GetUserByIdAsync(UserId);
                if (groupType == "online" || groupType == "video")
                {
                    UserGroup userGroup = new UserGroup()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        IsActive = true,
                        OtherLangs="",

                        GroupId = requestBody.ForeignKeyId,
                        UserId = UserId,
                    };
                    addResult = await _UserGroupRep.AddUserGroupsAsync(new List<UserGroup>() { userGroup});
                }

                else
                {
                    PreRegistration PreRegistration = new PreRegistration()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        Email = userRow.Result.Email,
                        FirstName = userRow.Result.FirstName,
                        LastName = userRow.Result.LastName,
                        PhoneNumber = userRow.Result.Username,

                        EducationalClass = null,
                        SchoolName = null,
                        FavoriteField = null,
                        RecognitionLevel = null,
                        ProgrammingSkillLevel = null,

                        ForeignKeyId = requestBody.ForeignKeyId,
                        EntityType = requestBody.EntityType ?? "",
                        TargetObjName = targetObjName,
                        RegistrationDate = DateTime.Now.ToShamsi(),
                        OtherLangs = null,
                        // Description = requestBody.Description,
                    };
                    addResult = await _PreRegistrationRep.AddPreRegistrationAsync(PreRegistration);
                }

                if (addResult.Status)
                {
                    var academyPhoneNum = await _settingRep.GetSettingRowAsync(0, "Contact_Phone_Value_EN");
                    var targetType = requestBody.EntityType.ToLower().Contains("event") ? "رویداد" : "گروه درسی";
                    dynamic targetObj = requestBody.EntityType.ToLower().Contains("event") ? await _EventRep.GetEventByIdAsync(requestBody.ForeignKeyId) :
                        await _GroupRep.GetGroupByIdAsync(requestBody.ForeignKeyId);
                    var targetName = requestBody.EntityType.ToLower().Contains("event") ? targetObj.Result.Title : targetObj.Result.Name;
                    var targetFee = requestBody.EntityType.ToLower().Contains("event") ? targetObj.Result.Fee.Value : targetObj.Result.Fee;
                    var registerDate = DateTime.Now.ToShamsiString().Split(' ')[0];
                    var registerTime = DateTime.Now.ToShamsiString().Split(' ')[1];

                    var infoMessage =
                        $@"دانشجو {userRow.Result.FirstName} {userRow.Result.LastName}
در تاریخ {registerDate}
ساعت {registerTime}
با پرداخت {targetFee} ریال در {targetType}
{targetName} ثبت نام شد";

                    bool sent = await ToolBox.SendSMSMessage(academyPhoneNum.Result.Value, infoMessage);

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
                else
                {
                    return BadRequest(addResult);
                }
            }

            return BadRequest(result);
        }

        [HttpGet("VerifyPayment")]
        public async Task<ActionResult<BitResultObject>> VerifyPayment(long PayId = 0,string? EntityType = "" , long? ForeignKeyId = 0,string? paymentToken = "",string? Authority ="", string? Status = "")
        {
            BitResultObject result = new BitResultObject();
            try
            {
                string targetObjName = "",groupType="";
                BitResultObject addResult;

                var paymentHistory = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(PayId);

                var UserId = User.GetCurrentUserId();

                var userRow = await _UserRep.GetUserByIdAsync(UserId);

                var invoice = await _onlinePayment.FetchAsync();

                dynamic theRow = EntityType.ToLower().Contains("event") ? await _EventRep.GetEventByIdAsync(ForeignKeyId.Value) :
    await _GroupRep.GetGroupByIdAsync(ForeignKeyId.Value);


                switch (EntityType.ToLower())
                {
                    default:
                    case "group":
                        {

                            if (theRow.Result == null)
                            {
                                result.Status = false;
                                result.ErrorMessage = "درخواست نامعتبر است";
                                return BadRequest(result);
                            }

                            targetObjName = theRow.Result.Name;
                            groupType = theRow.Result.GroupType;

                        }
                        break;
                    case "event":
                        {

                            if (theRow.Result == null)
                            {
                                result.Status = false;
                                result.ErrorMessage = "درخواست نامعتبر است";
                                return BadRequest(result);
                            }

                            targetObjName = theRow.Result.Title;


                        }
                        break;
                }

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
                    if (groupType == "online" || groupType == "video")
                    {
                        UserGroup userGroup = new UserGroup()
                        {
                            CreateDate = DateTime.Now.ToShamsi(),
                            UpdateDate = DateTime.Now.ToShamsi(),
                            IsActive = true,
                            OtherLangs = "",

                            GroupId = ForeignKeyId.Value,
                            UserId = UserId,
                        };
                        addResult = await _UserGroupRep.AddUserGroupsAsync(new List<UserGroup>() { userGroup });
                    }
                    else
                    {
                        PreRegistration PreRegistration = new PreRegistration()
                        {
                            CreateDate = DateTime.Now.ToShamsi(),
                            UpdateDate = DateTime.Now.ToShamsi(),
                            Email = userRow.Result.Email,
                            FirstName = userRow.Result.FirstName,
                            LastName = userRow.Result.LastName,
                            PhoneNumber = userRow.Result.Username,

                            EducationalClass = null,
                            SchoolName = null,
                            FavoriteField = null,
                            RecognitionLevel = null,
                            ProgrammingSkillLevel = null,
                            SocialAddress = null,

                            ForeignKeyId = ForeignKeyId ?? 0,
                            EntityType = EntityType ?? "",
                            TargetObjName = targetObjName,
                            IsActive = true,
                            RegistrationDate = DateTime.Now.ToShamsi(),
                            OtherLangs = null,
                            // Description = requestBody.Description,
                        };
                       addResult = await _PreRegistrationRep.AddPreRegistrationAsync(PreRegistration);
                    }
                      

                    if (addResult.Status)
                    {
                        var academyPhoneNum = await _settingRep.GetSettingRowAsync(0, "Contact_Phone_Value_EN");
                        var targetType = EntityType.ToLower().Contains("event") ? "رویداد" : "گروه درسی";
                        var targetName = EntityType.ToLower().Contains("event") ? theRow.Result.Title : theRow.Result.Name;
                        var targetFee = EntityType.ToLower().Contains("event") ? theRow.Result.Fee.Value : theRow.Result.Fee;
                        var registerDate = DateTime.Now.ToShamsiString().Split(' ')[0];
                        var registerTime = DateTime.Now.ToShamsiString().Split(' ')[1];

                        var infoMessage = 
                            $@"دانشجو {userRow.Result.FirstName} {userRow.Result.LastName}
در تاریخ {registerDate}
ساعت {registerTime}
با پرداخت {targetFee} در {targetType}
{targetName} ثبت نام شد";

                        bool sent = await ToolBox.SendSMSMessage(academyPhoneNum.Result.Value,infoMessage);

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
                    result.Status = false;
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
