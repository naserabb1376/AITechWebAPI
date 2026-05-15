namespace AITechDATA.Domain
{
    public class Software : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Note { get; set; }
        public string? DownloadUrl { get; set; }

        public long? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
