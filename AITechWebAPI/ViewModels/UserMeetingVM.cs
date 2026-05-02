using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ 
    public class UserMeetingVM : BaseVM
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        public long MeetingId { get; set; }

        public DateTime MeetingDate { get; set; }
        public TimeSpan MeetingStartTime { get; set; } // ساعت شروع
        public string MeetingTitle { get; set; }
        public string MeetingStatus { get; set; }
        public string MeetingOrganizer { get; set; }
        public string FullName { get; set; }

    }
}