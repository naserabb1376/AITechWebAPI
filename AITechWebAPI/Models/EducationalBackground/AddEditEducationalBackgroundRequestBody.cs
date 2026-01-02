using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.EducationalBackground
{
    public class AddEditEducationalBackgroundRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        [Display(Name = "رشته تحصیلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string StudyField { get; set; } // رشته تحصیلی

        [Display(Name = "مقطع تحصیلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EducationalGrade { get; set; } // مقطع تحصیلی

        [Display(Name = "توضیحات")]
        public string? Description { get; set; } //


    }
}
