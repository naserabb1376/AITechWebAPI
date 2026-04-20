using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class BookVM : BaseVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? AuthorName { get; set; }
        public string? Note { get; set; }
        public long? CategoryId { get; set; } // کلید خارجی به Category
        public string? CategoryName { get; set; } // ارتباط با Category

    }
}