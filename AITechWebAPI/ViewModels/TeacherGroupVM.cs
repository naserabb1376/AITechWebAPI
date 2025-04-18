using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class TeacherGroupVM
    {
        public string Name { get; set; } // نام گروه
        public string DayOfWeek { get; set; } // روز برگزاری (مثلاً شنبه)
        public TimeSpan StartTime { get; set; } // ساعت شروع
        public TimeSpan EndTime { get; set; } // ساعت پایان
        public decimal Fee { get; set; } // هزینه گروه
        public GroupStatus Status { get; set; } // وضعیت گروه (پیش‌ثبت‌نام، در حال اجرا، پایان یافته)
        public string FullName { get; set; } // کلید خارجی به استاد
    }
}
