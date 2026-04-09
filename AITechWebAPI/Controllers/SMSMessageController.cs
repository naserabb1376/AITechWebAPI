using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models.Public;
using AITechWebAPI.Models.SMSMessage;
using AITechWebAPI.Tools;
using AITechWebAPI.Validations;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NobatPlusDATA.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ServiceModel.Channels;
using System.Text;
using static AITechWebAPI.Tools.ToolBox;

namespace AITechAPI.Controllers
{
    [Route("SMSMessage")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    // [CheckRoleBase(new[] { (int)BaseRole.MiddleAdmin, (int)BaseRole.GeneralAdmin })]

    public class SMSMessageController : ControllerBase
    {
        ISMSMessageRep _SMSMessageRep;
        ILogRep _logRep;
        IUserRep _userRep;
        IJobRequestRep _jobRep;
        private readonly IMapper _mapper;


        public SMSMessageController(ISMSMessageRep SMSMessageRep,IUserRep userRep,ILogRep logRep, IMapper mapper,IJobRequestRep requestRep)
        {
           _SMSMessageRep = SMSMessageRep;
           _logRep = logRep;
            _userRep = userRep;
            _mapper = mapper;
            _jobRep = requestRep;
        }

        [HttpPost("GetAllSMSMessages_Base")]
        public async Task<ActionResult<ListResultObject<SMSMessageVM>>> GetAllSMSMessages_Base(GetSMSMessageListRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SMSMessageRep.GetAllSMSMessagesAsync(requestBody.UserId,requestBody.PageIndex,requestBody.PageSize,requestBody.SearchText,requestBody.SortQuery);
            if (result.Status)
            {
                var resultVM = _mapper.Map<ListResultObject<SMSMessageVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("GetSMSMessageById_Base")]
        public async Task<ActionResult<RowResultObject<SMSMessageVM>>> GetSMSMessageById_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SMSMessageRep.GetSMSMessageByIdAsync(requestBody.ID);
            if (result.Status)
            {
                var resultVM = _mapper.Map<RowResultObject<SMSMessageVM>>(result);
                return Ok(resultVM);
            }
            return BadRequest(result);
        }

        [HttpPost("ExistSMSMessage_Base")]
        public async Task<ActionResult<BitResultObject>> ExistSMSMessage_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SMSMessageRep.ExistSMSMessageAsync(requestBody.ID);
            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("AddSMSMessage_Base")]
        public async Task<ActionResult<BitResultObject>> AddSMSMessage_Base(AddEditSMSMessageRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var validPhoneNumber = await _userRep.ExistUserAsync(requestBody.PhoneNumber, "username");

            if (requestBody.PhoneNumberExists)
            {
                if (!validPhoneNumber.Status && string.IsNullOrEmpty(validPhoneNumber.ErrorMessage))
                {
                    result.Status = validPhoneNumber.Status;
                    result.ErrorMessage = "این کاربر در سیستم وجود ندارد";
                    return BadRequest(result);
                }
            }

            else
            {
                if (validPhoneNumber.Status)
                {
                    result.Status = !validPhoneNumber.Status;
                    result.ErrorMessage = "شماره تماس تکراری است";
                    return BadRequest(result);
                }
            }

            var sentstatus = await ToolBox.SendSMSMessage(requestBody.PhoneNumber,requestBody.Message);
          

            SMSMessage SMSMessage = new SMSMessage()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                PhoneNumber = requestBody.PhoneNumber,
                UserID = validPhoneNumber.ID > 0 ? validPhoneNumber.ID : null,
                Message = requestBody.Message,
                SentDate = string.IsNullOrEmpty(requestBody.SentDate) ? DateTime.Now.ToShamsi() : requestBody.SentDate.StringToDate().Value,
                OtherLangs = requestBody.OtherLangs ?? "",
                SentStatus = sentstatus,
            };
             result = await _SMSMessageRep.AddSMSMessageAsync(SMSMessage);
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


                if (sentstatus)
                {
                    result.ErrorMessage = $"پیامک ارسال شد";
                    return Ok(result);
                }
                else
                {
                    result.ErrorMessage = $"در ارسال پیامک مشکلی بوجود آمد";
                }
            }
            return BadRequest(result);
        }

//        [HttpPost("SendGroupInviteSMSMessage_Base")]
//        public async Task<ActionResult<BitResultObject>> SendGroupInviteSMSMessage_Base()
//        {
//            var result = new BitResultObject();
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }

//            string messageBody = $@"*شرکت‌کننده محترم،*

//ضمن قدردانی از درخواست همکاری صمیمانه شما، خواهشمند است از میان ساعت‌های اعلام‌شده در سایت، زمان مطلوب خود را برای انجام مصاحبه انتخاب فرمایید.

//برای  انتخاب ساعت های مصاحبه وارد سایت آموزشگاه آیتک شده و در صفحه ارسال رزومه بخش گرفتن نوبت را پر کنید.

//از توجه، همکاری و همراهی ارزشمند شما صمیمانه سپاسگزاریم.

//مرکز تخصصی تحقیقات و آموزش هوش مصنوعی آیتک

//لغو 11";

//           var jobRequests = await _jobRep.GetAllJobRequestsAsync(1,0);

//            int allCount = jobRequests.TotalCount;
//            int sentCount = 0;

//            foreach (var item in jobRequests.Results)
//            {
                
//                var existUser = await _userRep.ExistUserAsync(item.PhoneNumber, "username");

//                bool sentstatus = await ToolBox.SendSMSMessage(item.PhoneNumber, messageBody);

//                SMSMessage SMSMessage = new SMSMessage()
//                {
//                    CreateDate = DateTime.Now.ToShamsi(),
//                    UpdateDate = DateTime.Now.ToShamsi(),
//                    PhoneNumber = item.PhoneNumber,
//                    UserID = existUser.ID > 0 ? existUser.ID : null,
//                    Message = messageBody,
//                    SentDate = DateTime.Now.ToShamsi(),
//                    OtherLangs =  "",
//                    SentStatus = sentstatus,
//            }
//            ;
//            result = await _SMSMessageRep.AddSMSMessageAsync(SMSMessage);
//            if (result.Status)
//            {
//                #region AddLog

//                Log log = new Log()
//                {
//                    CreateDate = DateTime.Now.ToShamsi(),
//                    UpdateDate = DateTime.Now.ToShamsi(),
//                    LogTime = DateTime.Now.ToShamsi(),
//                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),

//                };
//                await _logRep.AddLogAsync(log);

//                #endregion
//            }


//            if (sentstatus)
//                {
//                    sentCount++;
//                }
                
//            }
//            result.ErrorMessage = $"{sentCount}/{allCount}  Sent!";
//            return Ok(result);
//        }

        [HttpPut("EditSMSMessage_Base")]
        public async Task<ActionResult<BitResultObject>> EditSMSMessage_Base(AddEditSMSMessageRequestBody requestBody)
        {
            var result = new BitResultObject();
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var validPhoneNumber = await _userRep.ExistUserAsync(requestBody.PhoneNumber, "username");

            var theRow = await _SMSMessageRep.GetSMSMessageByIdAsync(requestBody.ID);
            if (!theRow.Status)
            {
                result.Status = theRow.Status;
                result.ErrorMessage = theRow.ErrorMessage;
            }

            SMSMessage SMSMessage = new SMSMessage()
            {
                CreateDate = theRow.Result.CreateDate,
                UpdateDate = DateTime.Now.ToShamsi(),
                ID = requestBody.ID,
                PhoneNumber = requestBody.PhoneNumber,
                UserID = validPhoneNumber.ID > 0 ? validPhoneNumber.ID : null,
                Message = requestBody.Message,
                SentDate = string.IsNullOrEmpty(requestBody.SentDate) ? DateTime.Now.ToShamsi() : requestBody.SentDate.StringToDate().Value,
                SentStatus = theRow.Result.SentStatus,
                OtherLangs = requestBody.OtherLangs ?? "",
            };
            result = await _SMSMessageRep.EditSMSMessageAsync(SMSMessage);
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

        [HttpDelete("DeleteSMSMessage_Base")]
        public async Task<ActionResult<BitResultObject>> DeleteSMSMessage_Base(GetRowRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }
            var result = await _SMSMessageRep.RemoveSMSMessageAsync(requestBody.ID);
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
