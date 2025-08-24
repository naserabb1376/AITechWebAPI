using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Attendance: جدول حضور و غیاب
    public class AttendanceVM : BaseVM
    {
        public long UserId { get; set; } // کلید خارجی به User
        public long SessionId { get; set; } // کلید خارجی به Session
        public bool IsPresent { get; set; }
        public string UserName { get; set; }

    }
}