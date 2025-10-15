using AITechDATA.Domain;
using NobatPlusDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class SMSMessageVM : BaseVM
    {
        public long UserID { get; set; }
        public string UserFullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool SentStatus { get; set; }


    }
}