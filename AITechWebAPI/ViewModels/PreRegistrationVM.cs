using AITechDATA.Domain;
using AITechDATA.Tools;

namespace AITechWebAPI.ViewModels
{
    public class PreRegistrationVM : BaseVM
    {
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")

        public string FullName { get; set; } // نام کامل دانشجو
        public string Email { get; set; } // ایمیل
        public string PhoneNumber { get; set; } // شماره تماس
        public DateTime RegistrationDate { get; set; } = DateTime.Now.ToShamsi(); // تاریخ ثبت‌نام
    }
}