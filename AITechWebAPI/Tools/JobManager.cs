
using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.Tools;

namespace AITechWebAPI.Tools
{
    public class JobManager
    {
        private readonly IPaymentHistoryRep _paymentHistoryRep;
        private readonly ISMSMessageRep _sMSMessageRep;
        private readonly INotificationRep _notificationRep;
        private readonly ILogRep _logRep;

        public JobManager(
            IPaymentHistoryRep paymentHistoryRep,
            ISMSMessageRep sMSMessageRep,
            INotificationRep notificationRep,
            ILogRep logRep)
        {
            _paymentHistoryRep = paymentHistoryRep;
            _sMSMessageRep = sMSMessageRep;
            _notificationRep = notificationRep;
            _logRep = logRep;
        }

        public async Task SendInstallmentRemindMessage(long paymentHistoryId,long userId)
        {
            var paymentHistory = await _paymentHistoryRep.GetPaymentHistoryByIdAsync(paymentHistoryId);

            if (paymentHistory.Result == null) return;


            string message = @$"
{paymentHistory.Result.User.FirstName} {paymentHistory.Result.User.LastName} عزیز!
امروز سررسید پرداخت قسط شماست
لطفا نسبت به پرداخت قسط خود اقدام نمایید
مجموعه آموزش هوش مصنوعی آیتک
";


            #region SendSMS

            bool sentstatus = await ToolBox.SendSMSMessage(paymentHistory.Result.User.Username, message);



            SMSMessage sMSMessage = new SMSMessage()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                PhoneNumber = paymentHistory.Result.User.Username,
                UserID = paymentHistory.Result.UserId,
                Message = message,
                SentDate = DateTime.Now.ToShamsi(),
                IsActive = true,
                SentStatus = sentstatus,
            };
            var smsresult = await _sMSMessageRep.AddSMSMessageAsync(sMSMessage);
            if (smsresult.Status)
            {
                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = "SendBookingRemindMessage",

                };
                await _logRep.AddLogAsync(log);

                #endregion
            }

            #endregion

            #region SendNotification

            Notification Notification = new Notification()
            {
                CreateDate = DateTime.Now.ToShamsi(),
                UpdateDate = DateTime.Now.ToShamsi(),
                UserId = paymentHistory.Result.UserId,
                IsActive = true,
                IsRead = false,
                SenderUserId = userId,
                NotificationPassLevel = 1,
                Message = message,
            };
            var notifresult = await _notificationRep.AddNotificationAsync(Notification);
            if (notifresult.Status)
            {
                #region AddLog

                Log log = new Log()
                {
                    CreateDate = DateTime.Now.ToShamsi(),
                    UpdateDate = DateTime.Now.ToShamsi(),
                    LogTime = DateTime.Now.ToShamsi(),
                    ActionName = "SendBookingRemindMessage",

                };
                await _logRep.AddLogAsync(log);

                #endregion
            }

            #endregion
        }

    }
}
