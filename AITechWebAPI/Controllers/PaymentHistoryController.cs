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
using Hangfire;
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
        ICourseRep _CourseRep;
        IUserRep _UserRep;
        IUserGroupRep _UserGroupRep;
        IPreRegistrationRep _PreRegistrationRep;
        IDiscountRep _discountRep;
        ISettingRep _settingRep;
        ILogRep _logRep;
        private readonly IMapper _mapper;


        public PaymentHistoryController(IPaymentHistoryRep PaymentHistoryRep,IGroupRep groupRep,IEventRep eventRep,ICourseRep courseRep,IUserRep userRep,IUserGroupRep userGroupRep,IPreRegistrationRep preRegistrationRep,IDiscountRep discountRep,ISettingRep settingRep,ILogRep logRep,IMapper mapper, IOnlinePayment onlinePayment)
        {
            _onlinePayment = onlinePayment;
            _PaymentHistoryRep = PaymentHistoryRep;
            _GroupRep = groupRep;
            _EventRep = eventRep;
            _CourseRep = courseRep;
            _UserRep = userRep;
            _UserGroupRep = userGroupRep;
            _PreRegistrationRep = preRegistrationRep;
            _discountRep = discountRep;
            _settingRep = settingRep;
           _logRep = logRep;
            _mapper = mapper;
        }

        private async Task<string> GetPaymentTargetNameAsync(string entityType, long foreignKeyId)
        {
            var normalizedEntityType = entityType?.ToLower() ?? "";

            if (normalizedEntityType.Contains("event"))
            {
                var targetObj = await _EventRep.GetEventByIdAsync(foreignKeyId);
                return targetObj.Result?.Title ?? "";
            }

            if (normalizedEntityType.Contains("course"))
            {
                var targetObj = await _CourseRep.GetCourseByIdAsync(foreignKeyId);
                return targetObj.Result?.Title ?? "";
            }

            var groupObj = await _GroupRep.GetGroupByIdAsync(foreignKeyId);
            return groupObj.Result?.Name ?? "";
        }




        [HttpPost("GetAllPaymentHistories_Base")]
        public async Task<ActionResult<ListResultObject<PaymentHistoryVM>>> GetAllPaymentHistories_Base(GetPaymentHistoryListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _PaymentHistoryRep.GetAllPaymentHistoriesAsync(requestBody.ForeignKeyId,requestBody.EntityType,requestBody.UserId,requestBody.DiscountId,requestBody.PaymentStatus,requestBody.HasDiscount,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            var result = await _PaymentHistoryRep.GetAllPaymentHistoriesAsync(requestBody.ForeignKeyId,requestBody.EntityType,requestBody.UserId,requestBody.DiscountId,requestBody.PayState,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
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

            string targetObjName = await GetPaymentTargetNameAsync(requestBody.EntityType, requestBody.ForeignKeyId);

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
                PaymentStatus = requestBody.PaymentStatus,
                IsInstallment = requestBody.IsInstallment,
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

            string targetObjName = await GetPaymentTargetNameAsync(requestBody.EntityType, requestBody.ForeignKeyId);


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
                IsInstallment = requestBody.IsInstallment,
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
            long? appliedDiscountId = null;
           BitResultObject addResult;
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var UserId = User.GetCurrentUserId();
            var RoleId = User.GetCurrentRoleId();

            switch (requestBody.EntityType.ToLower())
            {
                default:
                case "group":
                    {
                         var theRow = await _GroupRep.GetGroupByIdAsync(requestBody.ForeignKeyId, UserId, RoleId);

                        if (theRow.Result == null)
                        {
                            result.Result = null;
                            result.Status = false ;
                            result.ErrorMessage = "درخواست نامعتبر است" ;
                            return BadRequest(result);
                        }
                        var currentRegistrationCount = await _GroupRep.GetGroupRegistrationCountAsync(requestBody.ForeignKeyId);
                        if (theRow.Result.GroupCapacity <= currentRegistrationCount)
                        {
                            result.Result = null;
                            result.Status = false;
                            result.ErrorMessage = "ظرفیت ثبت نام این گروه تکمیل شده است";
                            return BadRequest(result);
                        }

                        rowAmount = theRow.Result.Fee;
                        targetObjName = theRow.Result.Name;
                       // groupType = theRow.Result.GroupType.ToLower();
                        groupType = theRow.Result.Course.Category.CategoryName;
                        discountedrowAmount = theRow.Result.DiscountedFee;

                    }
                    break;
                case "event":
                    {
                         var theRow = await _EventRep.GetEventByIdAsync(requestBody.ForeignKeyId, UserId, RoleId);

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
                case "paymenthistory":
                    {
                        var theRow = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(requestBody.ForeignKeyId);

                        if (theRow.Result == null)
                        {
                            result.Result = null;
                            result.Status = false;
                            result.ErrorMessage = "درخواست نامعتبر است";
                            return BadRequest(result);
                        }
                        var installments = theRow.Result.PaymentInstallments.Where(i => !i.IsPaid).OrderBy(x => x.InstallmentNumber).Take(requestBody.InstallmentCount).ToList();
                        if (installments.Any(i=> !i.PayAllowed))
                        {
                            result.Result = null;
                            result.Status = false;
                            result.ErrorMessage = "شما مجاز به پرداخت این افساط نیستید";
                            return BadRequest(result);

            if (discountedrowAmount < rowAmount)
            {
                var automaticDiscount = await GetApplicableDiscountAsync(UserId, RoleId, requestBody.EntityType, requestBody.ForeignKeyId);
                if (automaticDiscount != null)
                {
                    appliedDiscountId = automaticDiscount.ID;
                    discountedrowAmount = CalculateDiscountedAmount(rowAmount, automaticDiscount);
                }
            }

                        }
                        var InstallmentFinePercentRow = await _settingRep.GetSettingRowAsync(0, "installmentfinepercent");
                        var InstallmentFinePercent = int.Parse(InstallmentFinePercentRow.Result.Value);
                        foreach (var item in installments)
                        {
                            if (item.DueDate < DateTime.Now)
                            {
                                item.Amount = Math.Round(item.Amount * InstallmentFinePercent / 100, 0);
                            }
                        }
                        rowAmount = installments.Sum(x=> x.Amount);
                        discountedrowAmount = rowAmount;


                    }
                    break;
            }

            if (discountedrowAmount == rowAmount && requestBody.DiscountId > 0)
            {
                var x = await _discountRep.GetDiscountByIdAsync(requestBody.DiscountId.Value);
                if (!x.Status || x.Result == null)
                {
                    result.Result = null;
                    result.Status = false;
                    result.ErrorMessage = "کد تخفیف معتبر نیست";
                    return BadRequest(result);
                }

                var groups = await _UserGroupRep.GetAllUserGroupsAsync(UserId,pageSize:0);
                var groupIds = groups.Results.Select(x => x.GroupId).ToList();
                bool installmentDiscount = !requestBody.IsInstallment || x.Result.EntityName.ToLower() == "paymenthistory";
                var validdiscount = ((installmentDiscount && x.Result.EntityName.ToLower() == requestBody.EntityType.ToLower() && x.Result.ForeignKeyId == requestBody.ForeignKeyId)
               || (string.IsNullOrEmpty(x.Result.EntityName) && x.Result.ForeignKeyId <= 0))
                && x.Result.ExpireDate >= DateTime.Now && x.Result.DiscountMaxUsage > (x.Result.PaymentHistories.Count(p => p.UserId == UserId && p.PaymentStatus)) && x.Result.IsActive
                && (x.Result.DiscountTargets.Any(t => (t.IsActive && (
                (t.TargetEntityName.ToLower() == "group" && (t.TargetId <= 0 || groupIds.Contains(t.TargetId))) ||
                (t.TargetEntityName.ToLower() == "role" && (t.TargetId <= 0 || t.TargetId == RoleId)) ||
                (t.TargetEntityName.ToLower() == "user" && (t.TargetId <= 0 || t.TargetId == UserId))
                ))));

                if (validdiscount)
                {
                    discountedrowAmount = CalculateDiscountedAmount(rowAmount, x.Result);
                    appliedDiscountId = requestBody.DiscountId;
                }
                else
                {
                    result.Result = null;
                    result.Status = false;
                    result.ErrorMessage = "کد تخفیف برای این پرداخت قابل استفاده نیست";
                    return BadRequest(result);
                }
            }

            rowAmount = discountedrowAmount;

            if (rowAmount > 0)
            {
                if (requestBody.EntityType.ToLower() != "paymenthistory")
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
                        DiscountId = appliedDiscountId,
                        IsActive = true,
                        IsInstallment = requestBody.IsInstallment,
                        //  Description = requestBody.Description,
                    };
                    addResult = await _PaymentHistoryRep.AddPaymentHistoryAsync(PaymentHistory);

                    if (PaymentHistory.IsInstallment)
                    {
                        var addedPayment = await _PaymentHistoryRep.GetPaymentHistoryByIdAsync(addResult.ID);

                        foreach (var item in addedPayment.Result.PaymentInstallments)
                        {
                            BackgroundJob.Schedule<JobManager>(
                              job => job.SendInstallmentRemindMessage(addResult.ID,UserId),
                              item.DueDate
                          );
                        }
                    }

                    
                }
                else
                {
                    addResult = new BitResultObject() { Status = true };
                }
               
                if (addResult.Status)
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

                    var zarInvoice = new ZarinPalInvoice(description: $"{addResult.ID} - {DateTime.Now.ToShamsi()}");
                    // var callbackUrl = Url.Action("VerifyPayment", "PaymentHistory", new { payId = Addresult.ID }, Request.Scheme);
                    var callbackUrl = $"https://aitechac.com/payment/verify?PayId={addResult.ID}&UserId={UserId}&InstallmentCount={requestBody.InstallmentCount}";

                    var invoice = await _onlinePayment.RequestAsync(invoice =>
                    {
                        invoice.SetCallbackUrl(callbackUrl)
                               .SetAmount(rowAmount)
                                .SetZarinPalData(zarInvoice)
                                .UseZarinPal();
                        invoice.SetTrackingNumber(addResult.ID);

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

                result.Result = null;
                result.Status = false;
                result.ErrorMessage = addResult.ErrorMessage;
                return BadRequest(result);
            }

            else
            {
                var userRow = await _UserRep.GetUserByIdAsync(UserId);
                var isFirstGroupRegistration = false;
                //if (groupType == "online" || groupType == "video")
                if (groupType.Contains( "آنلاین") || groupType.Contains( "آفلاین"))
                {
                    var userGroupsBeforeRegister = await _UserGroupRep.GetAllUserGroupsAsync(UserId, pageSize: 0);
                    isFirstGroupRegistration = !userGroupsBeforeRegister.Results.Any(x => x.IsActive);

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
                        PaymentFinished = true,
                        IsActive = true,

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
                    PaymentHistory PaymentHistory = new PaymentHistory()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        ForeignKeyId = requestBody.ForeignKeyId,
                        EntityType = requestBody.EntityType,
                        TargetObjName = targetObjName,
                        Amount = 0,
                        UserId = UserId,
                        PaymentDate = DateTime.Now.ToShamsi(),
                        PaymentStatus = true,
                        DiscountId = appliedDiscountId,
                        IsActive = true,
                    };
                    var paymentSaveResult = await _PaymentHistoryRep.AddPaymentHistoryAsync(PaymentHistory);
                    if (!paymentSaveResult.Status)
                    {
                        result.Result = null;
                        result.Status = false;
                        result.ErrorMessage = paymentSaveResult.ErrorMessage;
                        return BadRequest(result);
                    }

                    if (requestBody.EntityType.ToLower().Contains("group") && isFirstGroupRegistration)
                    {
                        await ApplyInvitationRewardIfNeededAsync(userRow.Result);
                    }
                    await DeactivateInvitationDiscountIfUsedAsync(appliedDiscountId);

                    var targetType = requestBody.EntityType.ToLower().Contains("event") ? "رویداد" : "گروه درسی";
                    dynamic targetObj = requestBody.EntityType.ToLower().Contains("event") ? await _EventRep.GetEventByIdAsync(requestBody.ForeignKeyId) :
                        await _GroupRep.GetGroupByIdAsync(requestBody.ForeignKeyId);
                    var targetName = requestBody.EntityType.ToLower().Contains("event") ? targetObj.Result.Title : targetObj.Result.Name;
                    var targetFee = requestBody.EntityType.ToLower().Contains("event") ? targetObj.Result.Fee.Value : targetObj.Result.Fee;
                    var registerDate = DateTime.Now.ToShamsiString().Split(' ')[0];
                    var registerTime = DateTime.Now.ToShamsiString().Split(' ')[1];
                    var paymentLine = BuildPaymentSmsLine(targetFee, PaymentHistory.Amount, PaymentHistory.DiscountId);

                    var infoMessage =
                        $@"دانشجو {userRow.Result.FirstName} {userRow.Result.LastName}
در تاریخ {registerDate}
ساعت {registerTime}
{paymentLine} در {targetType}
{targetName} ثبت نام شد";

                    var academyPhoneNum = await _settingRep.GetSettingRowAsync(0, "Contact_Phone_Value_EN");
                    var phoneNumbers = new List<string>() { academyPhoneNum.Result.Value };
                    if (requestBody.EntityType.ToLower().Contains("group"))
                    {
                        phoneNumbers.Add(targetObj.Result.Teacher.Username);

                        if (targetObj.Result.Course.Category.CategoryName.Contains("دانش آموزی"))
                        {
                            var eduAdmins = await _UserRep.GetAllUsersAsync(RoleIds: new List<long>() { (long)BaseRole.EduAdmin }, pageSize: 0);

                            phoneNumbers.AddRange(eduAdmins.Results.Select(x => x.Username).ToList());

                        }
                    }
                    foreach (var item in phoneNumbers)
                    {
                        bool sent = await ToolBox.SendSMSMessage(item, infoMessage);
                    }
                   

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

                result.Status = true;
                result.ErrorMessage = "";
                result.Result.PayGatewayUrl = null;
                return Ok(result);
            }

            return BadRequest(result);
        }

        private async Task<Discount?> GetApplicableDiscountAsync(long userId, long roleId, string entityType, long foreignKeyId)
        {
            var groups = await _UserGroupRep.GetAllUserGroupsAsync(userId, pageSize: 0);
            var groupIds = groups.Results.Where(x => x.IsActive).Select(x => x.GroupId).ToList();
            var discounts = await _discountRep.GetAllDiscountsAsync(pageSize: 0);

            return discounts.Results
                .Where(x => IsApplicableDiscount(x, userId, roleId, groupIds, entityType, foreignKeyId))
                .OrderByDescending(x => x.DiscountPercent)
                .ThenByDescending(x => x.DiscountAmount)
                .FirstOrDefault();
        }

        private static bool IsApplicableDiscount(Discount discount, long userId, long roleId, List<long> groupIds, string entityType, long foreignKeyId)
        {
            var entityMatches =
                (!string.IsNullOrEmpty(discount.EntityName) &&
                 discount.EntityName.ToLower() == entityType.ToLower() &&
                 discount.ForeignKeyId == foreignKeyId) ||
                (string.IsNullOrEmpty(discount.EntityName) && discount.ForeignKeyId <= 0);

            return entityMatches
                   && discount.ExpireDate >= DateTime.Now
                   && discount.DiscountMaxUsage > discount.PaymentHistories.Count(p => p.UserId == userId && p.PaymentStatus)
                   && discount.IsActive
                   && !discount.CodeRequired
                   && discount.DiscountTargets.Any(t => t.IsActive && (
                       (t.TargetEntityName.ToLower() == "group" && (t.TargetId <= 0 || groupIds.Contains(t.TargetId))) ||
                       (t.TargetEntityName.ToLower() == "role" && (t.TargetId <= 0 || t.TargetId == roleId)) ||
                       (t.TargetEntityName.ToLower() == "user" && (t.TargetId <= 0 || t.TargetId == userId))));
        }

        private static decimal CalculateDiscountedAmount(decimal amount, Discount discount)
        {
            var amountBased = discount.DiscountAmount > 0
                ? amount - discount.DiscountAmount
                : amount;
            var percentBased = discount.DiscountPercent > 0
                ? amount - (amount * discount.DiscountPercent / 100m)
                : amount;

            return Math.Max(0, decimal.Min(amountBased, percentBased));
        }

        private static string BuildPaymentSmsLine(decimal originalAmount, decimal paidAmount, long? discountId)
        {
            if (discountId.HasValue && discountId.Value > 0)
            {
                return $"با تخفیف و پرداخت {FormatSmsAmount(paidAmount)} ریال (مبلغ اصلی {FormatSmsAmount(originalAmount)} ریال)";
            }

            return $"با پرداخت {FormatSmsAmount(paidAmount)} ریال";
        }

        private static string FormatSmsAmount(decimal amount)
        {
            return amount.ToString("#,0");
        }

        private async Task ApplyInvitationRewardIfNeededAsync(User user)
        {
            if (user == null || user.InviterUserId == null || user.InvitationRewardApplied)
            {
                return;
            }

            var inviterDiscountPercentRow = await _settingRep.GetSettingRowAsync(0, "inviterdiscountpercent");
            var invitedDiscountPercentRow = await _settingRep.GetSettingRowAsync(0, "inviteddiscountpercent");
            var inviteDiscountDurationRow = await _settingRep.GetSettingRowAsync(0, "invitediscountduration");
            var inviteDiscountEntitiesRow = await _settingRep.GetSettingRowAsync(0, "invitediscountentities");
            var inviteDiscountMaxUsageRow = await _settingRep.GetSettingRowAsync(0, "invitediscountmaxusage");

            if (!inviterDiscountPercentRow.Status || !invitedDiscountPercentRow.Status || !inviteDiscountDurationRow.Status ||
                !inviteDiscountEntitiesRow.Status || !inviteDiscountMaxUsageRow.Status)
            {
                return;
            }

            var inviterDiscountPercent = int.Parse(inviterDiscountPercentRow.Result.Value);
            var invitedDiscountPercent = int.Parse(invitedDiscountPercentRow.Result.Value);
            var inviteDiscountDuration = int.Parse(inviteDiscountDurationRow.Result.Value);
            var inviteDiscountMaxUsage = int.Parse(inviteDiscountMaxUsageRow.Result.Value);
            var inviteDiscountEntities = inviteDiscountEntitiesRow.Result.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var rewardApplied = true;
            foreach (var entity in inviteDiscountEntities)
            {
                rewardApplied &= await AddOrChargeInvitationDiscountAsync(user.InviterUserId.Value, entity, inviterDiscountPercent, inviteDiscountDuration, inviteDiscountMaxUsage, $"inviter in invitation of {user.FirstName} {user.LastName}");
                rewardApplied &= await AddInvitationDiscountAsync(user.ID, entity, invitedDiscountPercent, inviteDiscountDuration, inviteDiscountMaxUsage, $"invited in invitation by user {user.InviterUserId}");
            }

            if (rewardApplied)
            {
                await _UserRep.MarkInvitationRewardAppliedAsync(user.ID);
            }
        }

        private async Task<bool> AddOrChargeInvitationDiscountAsync(long userId, string entity, int percent, int durationDays, int maxUsage, string description)
        {
            var discounts = await _discountRep.GetAllDiscountsAsync(entityName: entity, creatorId: userId, pageSize: 0, searchText: "invitation");
            var activeDiscount = discounts.Results
                .Where(x => x.IsActive &&
                            !x.CodeRequired &&
                            x.ExpireDate >= DateTime.Now &&
                            x.DiscountTargets.Any(t => t.IsActive && t.TargetEntityName.ToLower() == "user" && t.TargetId == userId) &&
                            x.DiscountMaxUsage > x.PaymentHistories.Count(p => p.UserId == userId && p.PaymentStatus))
                .OrderByDescending(x => x.DiscountPercent)
                .FirstOrDefault();

            if (activeDiscount == null)
            {
                return await AddInvitationDiscountAsync(userId, entity, percent, durationDays, maxUsage, description);
            }

            activeDiscount.DiscountPercent += percent;
            activeDiscount.UpdateDate = DateTime.Now.ToShamsi();
            activeDiscount.ExpireDate = DateTime.Now.AddDays(durationDays);
            var result = await _discountRep.EditDiscountAsync(activeDiscount);
            return result.Status;
        }

        private async Task<bool> AddInvitationDiscountAsync(long userId, string entity, int percent, int durationDays, int maxUsage, string description)
        {
            Discount discount = new Discount()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                OtherLangs = null,
                IsActive = true,
                DiscountAmount = 0,
                DiscountCode = "".GenerateDiscountCode(),
                CodeRequired = false,
                Description = description,
                CreatorId = userId,
                EntityName = entity,
                ForeignKeyId = 0,
                DiscountMaxUsage = maxUsage,
                ExpireDate = DateTime.Now.AddDays(durationDays),
                DiscountPercent = percent,
                DiscountTargets = new List<DiscountTarget>()
                {
                    new DiscountTarget()
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        OtherLangs = null,
                        IsActive = true,
                        TargetEntityName = "user",
                        TargetId = userId,
                    }
                },
            };

            var result = await _discountRep.AddDiscountAsync(discount);
            return result.Status;
        }

        private async Task DeactivateInvitationDiscountIfUsedAsync(long? discountId)
        {
            if (discountId == null || discountId <= 0)
            {
                return;
            }

            var discountRow = await _discountRep.GetDiscountByIdAsync(discountId.Value);
            if (!discountRow.Status || discountRow.Result == null || string.IsNullOrEmpty(discountRow.Result.Description) ||
                !discountRow.Result.Description.Contains("invitation"))
            {
                return;
            }

            discountRow.Result.DiscountPercent = 0;
            discountRow.Result.DiscountAmount = 0;
            discountRow.Result.IsActive = false;
            discountRow.Result.UpdateDate = DateTime.Now.ToShamsi();
            await _discountRep.EditDiscountAsync(discountRow.Result);
        }

        [HttpGet("VerifyPayment")]
        public async Task<ActionResult<BitResultObject>> VerifyPayment(long PayId = 0,int InstallmentCount = 0, string? paymentToken = "",string? Authority ="", string? Status = "")
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

                dynamic targetObj;


                switch (paymentHistory.Result.EntityType.ToLower())
                {
                    default:
                    case "group":
                        {
                            targetObj = await _GroupRep.GetGroupByIdAsync(paymentHistory.Result.ForeignKeyId);

                            if (targetObj.Result == null)
                            {
                                result.Status = false;
                                result.ErrorMessage = "درخواست نامعتبر است";
                                return BadRequest(result);
                            }

                            targetObjName = targetObj.Result.Name;
                            //groupType = targetObj.Result.GroupType;
                            groupType = targetObj.Result.Course.Category.CategoryName.ToLower();
                        }
                        break;
                    case "event":
                        {
                            targetObj = await _EventRep.GetEventByIdAsync(paymentHistory.Result.ForeignKeyId);

                            if (targetObj.Result == null)
                            {
                                result.Status = false;
                                result.ErrorMessage = "درخواست نامعتبر است";
                                return BadRequest(result);
                            }

                            targetObjName = targetObj.Result.Title;


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
                    if (paymentHistory.Result.IsInstallment)
                    {
                        var installments = paymentHistory.Result.PaymentInstallments.Where(i => !i.IsPaid).OrderBy(x => x.InstallmentNumber).Take(InstallmentCount).ToList();

                        foreach (var inst in installments)
                        {
                            inst.IsPaid = true;
                            inst.PaidDate = DateTime.Now.ToShamsi();
                            inst.UpdateDate = DateTime.Now.ToShamsi();
                        }

                        if (paymentHistory.Result.PaymentInstallments.All(i => i.IsPaid))
                        {
                            paymentHistory.Result.PaymentStatus = true;
                        }
                    }


                    var isFirstGroupRegistration = false;
                    // if (groupType == "online" || groupType == "video")
                    if (groupType.Contains("آنلاین") || groupType.Contains("آفلاین"))
                    {
                        var userGroupsBeforeRegister = await _UserGroupRep.GetAllUserGroupsAsync(UserId, pageSize: 0);
                        isFirstGroupRegistration = !userGroupsBeforeRegister.Results.Any(x => x.IsActive);

                        UserGroup userGroup = new UserGroup()
                        {
                            CreateDate = DateTime.Now.ToShamsi(),
                            UpdateDate = DateTime.Now.ToShamsi(),
                            IsActive = true,
                            OtherLangs = "",
                        if (paymentHistory.Result.PaymentStatus)
                        {
                            UserGroup userGroup = new UserGroup()
                            {
                                CreateDate = DateTime.Now.ToShamsi(),
                                UpdateDate = DateTime.Now.ToShamsi(),
                                IsActive = true,
                                OtherLangs = "",

                                GroupId = paymentHistory.Result.ForeignKeyId,
                                UserId = UserId,
                            };
                            addResult = await _UserGroupRep.AddUserGroupsAsync(new List<UserGroup>() { userGroup });
                        }
                        else
                        {
                            addResult = new BitResultObject() { Status= true };
                        }
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
                            PaymentFinished = paymentHistory.Result.PaymentStatus,
                            EducationalClass = null,
                            SchoolName = null,
                            FavoriteField = null,
                            RecognitionLevel = null,
                            ProgrammingSkillLevel = null,
                            SocialAddress = null,

                            ForeignKeyId = paymentHistory.Result.ForeignKeyId,
                            EntityType = paymentHistory.Result.EntityType,
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
                        if (EntityType.ToLower().Contains("group") && isFirstGroupRegistration)
                        {
                            await ApplyInvitationRewardIfNeededAsync(userRow.Result);
                        }
                        await DeactivateInvitationDiscountIfUsedAsync(paymentHistory.Result.DiscountId);

                        var targetType = EntityType.ToLower().Contains("event") ? "رویداد" : "گروه درسی";
                        var targetName = EntityType.ToLower().Contains("event") ? targetObj.Result.Title : targetObj.Result.Name;
                        var targetFee = EntityType.ToLower().Contains("event") ? targetObj.Result.Fee.Value : targetObj.Result.Fee;
                        var targetType = paymentHistory.Result.EntityType.ToLower().Contains("event") ? "رویداد" : "گروه درسی";
                        var targetName = paymentHistory.Result.EntityType.ToLower().Contains("event") ? targetObj.Result.Title : targetObj.Result.Name;
                        var targetFee = paymentHistory.Result.EntityType.ToLower().Contains("event") ? targetObj.Result.Fee.Value : targetObj.Result.Fee;
                        var registerDate = DateTime.Now.ToShamsiString().Split(' ')[0];
                        var registerTime = DateTime.Now.ToShamsiString().Split(' ')[1];
                        var paymentLine = BuildPaymentSmsLine(targetFee, paymentHistory.Result.Amount, paymentHistory.Result.DiscountId);

                        var infoMessage = 
                            $@"دانشجو {userRow.Result.FirstName} {userRow.Result.LastName}
در تاریخ {registerDate}
ساعت {registerTime}
{paymentLine} در {targetType}
{targetName} ثبت نام شد";

                        var academyPhoneNum = await _settingRep.GetSettingRowAsync(0, "Contact_Phone_Value_EN");
                        var phoneNumbers = new List<string>() { academyPhoneNum.Result.Value };
                        if (paymentHistory.Result.EntityType.ToLower().Contains("group"))
                        {
                            phoneNumbers.Add(targetObj.Result.Teacher.Username);

                            if (targetObj.Result.Course.Category.CategoryName.Contains("دانش آموزی"))
                            {
                                var eduAdmins = await _UserRep.GetAllUsersAsync(RoleIds: new List<long>() { (long)BaseRole.EduAdmin }, pageSize: 0);

                                phoneNumbers.AddRange(eduAdmins.Results.Select(x => x.Username).ToList());

                            }
                        }
                        foreach (var item in phoneNumbers)
                        {
                            bool sent = await ToolBox.SendSMSMessage(item, infoMessage);
                        }

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
