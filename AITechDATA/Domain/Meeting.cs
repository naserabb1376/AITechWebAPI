using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    // Meeting: جدول جلسه کاری
    public class Meeting : BaseEntity
    {
        public DateTime MeetingDate { get; set; }
        public TimeSpan MeetingStartTime { get; set; } // ساعت شروع
        public string MeetingTitle { get; set; }
        public string MeetingAttendees { get; set; }
        public string MeetingStatus { get; set; }
        public string MeetingOrganizer { get; set; }

        public List<Minutes> Minutes { get; set; }
        public ICollection<UserMeeting> UserMeetings { get; set; }
    }
}