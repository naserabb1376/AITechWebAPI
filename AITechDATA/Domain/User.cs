
  using System.ComponentModel.DataAnnotations;
using AiTech.Domains;
using Microsoft.AspNetCore.Identity;

namespace AITechDATA.Domain
{
    // User: جدول کاربران
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NationalCode { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // هش رمز عبور
        public long RoleId { get; set; } // کلید خارجی به Role
        public Role Role { get; set; } // ارتباط با Role
        public long? AddressId { get; set; } // کلید خارجی به Address
        public Address? Address { get; set; } // ارتباط با Address
        public ICollection<TeacherResume> TeacherResumes { get; set; } // رزومه‌های مرتبط با استاد
        //public string ParentInfo { get; set; } // اطلاعات والدین برای دانش‌آموزان
        public ICollection<PaymentHistory> PaymentHistories { get; set; } // ارتباط یک به چند با PaymentHistory
        //public ICollection<Course> CoursesTaught { get; set; } // دوره‌هایی که تدریس می‌کند
        //public ICollection<Course> CoursesEnrolled { get; set; } // دوره‌هایی که شرکت کرده است
        public ICollection<UserCourse> UserCourses { get; set; } // دوره‌هایی که شرکت یا تدریس کرده است
        public ICollection<UserGroup> UserGroups { get; set; } 
        public ICollection<LoginMethod> LoginMethods { get; set; } 
        public ICollection<Assignment> Assignments { get; set; } // تمرین‌های ارسال شده توسط کاربر
        public ICollection<Attendance> Attendances { get; set; } // حضور و غیاب هر کاربر
        public ICollection<Notification> Notifications { get; set; } // اعلان ‌های ارسال شده توسط کاربر
        public ICollection<Event> Events { get; set; } // رویداد‌های ثبت شده توسط کاربر
        public ICollection<TicketMessage> TicketMessages { get; set; } // پاسخ تیکت‌های ثبت شده توسط مدیر
        public ICollection<News> News { get; set; } // اخبار ثبت شده توسط کاربر
        public StudentDetails StudentDetails { get; set; } // جزئیات دانش‌آموز
    }
}