using AITechWebAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.InterviewTime
{
    public class AddEditInterviewTimeRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "تاریخ مصاحبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InterviewDate { get; set; }

        [Display(Name = "زمان شروع مصاحبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InterviewStartTime { get; set; }

        [Display(Name = "زمان پایان مصاحبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InterviewEndTime { get; set; }

        [Display(Name = "کد درخواست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long JobRequestId { get; set; }

        [Display(Name = "شرح")]
        public string? Description { get; set; } = "";

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
