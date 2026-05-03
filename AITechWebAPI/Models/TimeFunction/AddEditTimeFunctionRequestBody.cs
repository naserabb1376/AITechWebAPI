using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TimeFunction
{
    public class AddEditTimeFunctionRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "تاریخ شروع کارکرد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TimeFunctionStartDate { get; set; }

        [Display(Name = "تاریخ پایان کارکرد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TimeFunctionEndDate { get; set; }

        [Display(Name = "توضیحات کارکرد")]
        public string? Description { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
