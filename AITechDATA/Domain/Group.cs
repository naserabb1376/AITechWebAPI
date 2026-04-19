using AiTech.Domains;
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
        public DateTime StartDate { get; set; } // تاریخ شروع
        public DateTime EndDate { get; set; } // تاریخ پایان
        public TimeSpan StartTime { get; set; } // ساعت شروع
        public TimeSpan? EndTime { get; set; } // ساعت پایان
        public decimal Fee { get; set; } // هزینه گروه
        public GroupStatus Status { get; set; } // وضعیت گروه (پیش‌ثبت‌نام، در حال اجرا، پایان یافته)
        public string GroupType { get; set; } // نوع برگزاری گروه (حضوری، غیر حضوری)
        public int GroupCapacity { get; set; } // ظرفیت ثبت نام گروه
        public int RegisterCount { get; set; } // تعداد ثبت نام گروه
        public string? Note { get; set; }
    //    public ICollection<PreRegistration> PreRegistrations { get; set; } // پیش‌ثبت‌نام‌های مرتبط با گروه
        public long TeacherId { get; set; } // کلید خارجی به استاد
        public User Teacher { get; set; } // ارتباط با استاد
        public ICollection<Session> Sessions { get; set; } // جلسات مرتبط با گروه
        public ICollection<UserGroup> Students { get; set; } // دانش‌آموزان ثبت‌نام‌شده در گروه
        public ICollection<PaymentHistory> PaymentHistories { get; set; } // وضعیت پرداخت ها در گروه
        public ICollection<GroupChatMessage> ChatMessages { get; set; }
    }

    public class GroupDto : BaseEntity
    {
        public string Name { get; set; } // نام گروه
        public long CourseId { get; set; } // کلید خارجی به Course
        public Course Course { get; set; } // ارتباط با Course
        public string DayOfWeek { get; set; } // روز برگزاری (مثلاً شنبه)
        public DateTime StartDate { get; set; } // تاریخ شروع
        public DateTime EndDate { get; set; } // تاریخ پایان
        public TimeSpan StartTime { get; set; } // ساعت شروع
        public TimeSpan? EndTime { get; set; } // ساعت پایان
        public decimal Fee { get; set; } // هزینه گروه
        public GroupStatus Status { get; set; } // وضعیت گروه (پیش‌ثبت‌نام، در حال اجرا، پایان یافته)
        public string GroupType { get; set; } // نوع برگزاری گروه (حضوری، غیر حضوری)
        public int GroupCapacity { get; set; } // ظرفیت ثبت نام گروه
        public int RegisterCount { get; set; } // تعداد ثبت نام گروه
        public string? Note { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountedFee { get; set; }

        public string? RegisterMode { get; set; } 


        public ICollection<Session> Sessions { get; set; } // جلسات مرتبط با گروه
                                                           //    public ICollection<PreRegistration> PreRegistrations { get; set; } // پیش‌ثبت‌نام‌های مرتبط با گروه
        public long TeacherId { get; set; } // کلید خارجی به استاد
        public User Teacher { get; set; } // ارتباط با استاد
        public ICollection<UserGroup> Students { get; set; } // دانش‌آموزان ثبت‌نام‌شده در گروه
        public ICollection<PaymentHistory> PaymentHistories { get; set; } // وضعیت پرداخت ها در گروه
        public ICollection<GroupChatMessage> ChatMessages { get; set; }
    }

    // تعریف وضعیت گروه به عنوان یک enum
    public enum GroupStatus
    {
        PreRegistration, // پیش‌ثبت‌نام
        Active,          // در حال اجرا
        Completed        // پایان یافته
    }
}