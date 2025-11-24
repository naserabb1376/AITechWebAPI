using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class MeetingVM : BaseVM
    {
        public DateTime MeetingDate { get; set; }
        public TimeSpan MeetingStartTime { get; set; } // ساعت شروع
        public string MeetingTitle { get; set; }
        public string MeetingAttendees { get; set; }
        public string MeetingStatus { get; set; }
        public string MeetingOrganizer { get; set; }

    }
}
