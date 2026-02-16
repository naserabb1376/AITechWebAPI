using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class GroupVM:BaseVM
    {
        public string Name { get; set; } // نام گروه
        public string DayOfWeek { get; set; } // روز برگزاری (مثلاً شنبه)
        public DateTime StartDate { get; set; } // تاریخ شروع
        public DateTime EndDate { get; set; } // تاریخ پایان
        public TimeSpan StartTime { get; set; } // ساعت شروع
        public TimeSpan? EndTime { get; set; } // ساعت پایان
        public decimal Fee { get; set; } // هزینه گروه
        public GroupStatus Status { get; set; } // وضعیت گروه (پیش‌ثبت‌نام، در حال اجرا، پایان یافته)
        public string GroupType { get; set; } // نوع برگزاری گروه (حضوری، غیر حضوری)
        public string TeacherName { get; set; } // کلید خارجی به استاد
        public long CourseId { get; set; } // کلید خارجی به Course
        public long TeacherId { get; set; } // کلید خارجی به Teacher
        public string CourseTitle { get; set; }

    }
}
