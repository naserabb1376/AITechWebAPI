using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    // JobRequest: جدول درخواست شغل
    public class JobRequest : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? NationalCode { get; set; }
        public string? FatherName { get; set; }
        public string RequestedPosition { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? UniversityName { get; set; }
        public string? LastAcademicLicense { get; set; }
        public string? EducationalLevel { get; set; }
        public string? EducationStatus { get; set; }
        public string? CourseTitle { get; set; } = "";
        public string? Description { get; set; } = ""; // توضیحات
    }
}