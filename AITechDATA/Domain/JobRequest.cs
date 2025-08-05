namespace AITechDATA.Domain
{
    // JobRequest: جدول درخواست شغل
    public class JobRequest : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RequestedPosition { get; set; }
        public string? CourseTitle { get; set; } = "";
        public string? Description { get; set; } = ""; // توضیحات
    }
}