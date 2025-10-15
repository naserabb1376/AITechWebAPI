using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.InterviewTime
{
    public class GetInterviewTimesInDayRequestBody
    {
        public int PageIndex { get; set; } = 1;

        [Display(Name = "اندازه صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int PageSize { get; set; }

        [Display(Name = "تاریخ مصاحبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InterviewDate { get; set; }

        [Display(Name = "زمان شروع مصاحبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InterviewStartTime { get; set; }

        [Display(Name = "زمان پایان مصاحبه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string InterviewEndTime { get; set; }

        [Display(Name = "مدت زمان مصاحبه (به دقیقه)")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, 60, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public int InterviewMinutes { get; set; }


    }
}
