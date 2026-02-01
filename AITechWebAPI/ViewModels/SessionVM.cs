using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Session: جدول جلسات دوره‌ها
    public class SessionVM : BaseVM
    {
        public long GroupId { get; set; } // کلید خارجی به Group
        public string GroupName { get; set; } // ارتباط با Group
        public string? Description { get; set; }
        public DateTime SessionDate { get; set; } // تاریخ برگزاری جلسه
    }
}