using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Course: جدول دوره‌ها
    public class CourseVM : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        public long CategoryId { get; set; } // کلید خارجی به Category
        public string CategoryName { get; set; } // ارتباط با Category

    }
}