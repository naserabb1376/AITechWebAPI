using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TeacherResume
{
    public class AddEditTeacherResumeRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان رزومه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } 

        [Display(Name = "شرح رزومه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "تاریخ کسب یا انجام")]
        public string? DateAchieved { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
