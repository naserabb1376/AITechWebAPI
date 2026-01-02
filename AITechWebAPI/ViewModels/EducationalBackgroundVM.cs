using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class EducationalBackgroundVM : BaseVM
    {
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; }
        public string StudyField { get; set; } // رشته تحصیلی
        public string EducationalGrade { get; set; } // مقطع تحصیلی
        public string? Description { get; set; } //

    }
}