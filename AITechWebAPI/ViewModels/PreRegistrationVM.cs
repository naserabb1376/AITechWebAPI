using AITechDATA.Domain;
using AITechDATA.Tools;

namespace AITechWebAPI.ViewModels
{
    public class PreRegistrationVM : BaseVM
    {
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } // ایمیل
        public string PhoneNumber { get; set; } // شماره تماس
        public DateTime RegistrationDate { get; set; } = DateTime.Now.ToShamsi(); // تاریخ ثبت‌نام

        public string? EducationalClass { get; set; } // پایه تحصیلی
        public string? SchoolName { get; set; } // نام مدرسه
        public string? FavoriteField { get; set; } // حوزه علاقمندی
        public string? RecognitionLevel { get; set; } // میزان آشنایی با حوزه علاقمندی
        public string? ProgrammingSkillLevel { get; set; } // میزان تسلط به زبان های برنامه نویسی
        public string? SocialAddress { get; set; } // آدرس شبک اجتماعی


    }
}