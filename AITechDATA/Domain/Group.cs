using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Group: جدول گروه‌ها
    public class Group : BaseEntity
    {
        public string Name { get; set; } // نام گروه
        public long CourseId { get; set; } // کلید خارجی به Course
        public Course Course { get; set; } // ارتباط با Course
        public string DayOfWeek { get; set; } // روز برگزاری (مثلاً شنبه)
        public TimeSpan StartTime { get; set; } // ساعت شروع
        public TimeSpan EndTime { get; set; } // ساعت پایان
        public decimal Fee { get; set; } // هزینه گروه
        public GroupStatus Status { get; set; } // وضعیت گروه (پیش‌ثبت‌نام، در حال اجرا، پایان یافته)
        public ICollection<Session> Sessions { get; set; } // جلسات مرتبط با گروه
        public ICollection<PreRegistration> PreRegistrations { get; set; } // پیش‌ثبت‌نام‌های مرتبط با گروه
        public long TeacherId { get; set; } // کلید خارجی به استاد
        public User Teacher { get; set; } // ارتباط با استاد
        public ICollection<User> Students { get; set; } // دانش‌آموزان ثبت‌نام‌شده در گروه
    }

    // تعریف وضعیت گروه به عنوان یک enum
    public enum GroupStatus
    {
        PreRegistration, // پیش‌ثبت‌نام
        Active,          // در حال اجرا
        Completed        // پایان یافته
    }
}