using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TimeBreak
{
    public class AddEditTimeBreakRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کارکرد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long TimeFunctionID { get; set; }

        [Display(Name = "زمان شروع استراحت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TimeBreakStartTime { get; set; }

        [Display(Name = "زمان پایان استراحت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TimeBreakEndTime { get; set; }

        [Display(Name = "توضیحات کارکرد")]
        public string? Description { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
