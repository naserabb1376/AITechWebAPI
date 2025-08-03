using AITechDATA.Domain;
using AITechDATA.Tools;

namespace AITechWebAPI.ViewModels
{
    public class PreRegistrationVM : BaseEntity
    {
        public long GroupId { get; set; } // کلید خارجی به Group
        public string GroupName { get; set; } // ارتباط با Group

        public string FullName { get; set; } // نام کامل دانشجو
        public string Email { get; set; } // ایمیل
        public string PhoneNumber { get; set; } // شماره تماس
        public DateTime RegistrationDate { get; set; } = DateTime.Now.ToShamsi(); // تاریخ ثبت‌نام
    }
}