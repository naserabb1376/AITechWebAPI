using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    // Award: جدول جایزه ها
    public class Award : BaseEntity
    {
        public string AwardTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Description { get; set; } = ""; // توضیحات
    }
}