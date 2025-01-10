using System.Text.RegularExpressions;

namespace AITechDATA.Domain
{
    // Course: جدول دوره‌ها
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; } // کلید خارجی به Category
        public Category Category { get; set; } // ارتباط با Category
        public ICollection<UserCourse> UserCourses { get; set; } // دوره‌هایی که شرکت یا تدریس کرده است

       public ICollection<Group> Groups { get; set; } // گروه‌های مرتبط با دوره

    }
}