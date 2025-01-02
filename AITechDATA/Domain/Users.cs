using System.ComponentModel.DataAnnotations;
using AiTech.Domains;
using Microsoft.AspNetCore.Identity;

namespace AITechDATA.Domain
{
    // User: جدول کاربران
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // هش رمز عبور
        public long RoleId { get; set; } // کلید خارجی به Role
        public Role Role { get; set; } // ارتباط با Role
        public ICollection<TeacherResume> TeacherResumes { get; set; } // رزومه‌های مرتبط با استاد
        public string ParentInfo { get; set; } // اطلاعات والدین برای دانش‌آموزان
        public ICollection<PaymentHistory> PaymentHistories { get; set; } // ارتباط یک به چند با PaymentHistory
        public ICollection<Course> CoursesTaught { get; set; } // دوره‌هایی که تدریس می‌کند
        public ICollection<Course> CoursesEnrolled { get; set; } // دوره‌هایی که شرکت کرده است
        public ICollection<Assignment> Assignments { get; set; } // تمرین‌های ارسال شده توسط کاربر
        public StudentDetails StudentDetails { get; set; } // جزئیات دانش‌آموز
    }
}