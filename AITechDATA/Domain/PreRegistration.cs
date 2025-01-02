using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class PreRegistration : BaseEntity
    {
        public long CourseId { get; set; } // کلید خارجی به Course
        public Course Course { get; set; } // ارتباط با Course
        public string FullName { get; set; } // نام کامل دانشجو
        public string Email { get; set; } // ایمیل
        public string PhoneNumber { get; set; } // شماره تماس
        public DateTime RegistrationDate { get; set; } = DateTime.Now; // تاریخ ثبت‌نام
    }
}