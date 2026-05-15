using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class SoftwareVM : BaseVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? DownloadUrl { get; set; }
        public long? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
