using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class PreRegistration : BaseEntity
    {
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } // ایمیل
        public string PhoneNumber { get; set; } // شماره تماس

        public string? EducationalClass { get; set; } // پایه تحصیلی
        public string? SchoolName { get; set; } // نام مدرسه
        public string? FavoriteField { get; set; } // حوزه علاقمندی
        public string? RecognitionLevel { get; set; } // میزان آشنایی با حوزه علاقمندی
        public string? ProgrammingSkillLevel { get; set; } // میزان تسلط به زبان های برنامه نویسی
        public string? SocialAddress { get; set; } // آدرس شبک اجتماعی

        public DateTime RegistrationDate { get; set; } = DateTime.Now.ToShamsi(); // تاریخ ثبت‌نام
    }
}