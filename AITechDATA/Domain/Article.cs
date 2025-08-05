using System.Text.RegularExpressions;

namespace AITechDATA.Domain
{
    // Article: جدول مقالات
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        public long CategoryId { get; set; } // کلید خارجی به Category
        public Category Category { get; set; } // ارتباط با Category

    }
}