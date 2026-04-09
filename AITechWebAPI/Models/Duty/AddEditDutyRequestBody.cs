using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Duty
{
    public class AddEditDutyRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "کد کاربر ارسال کننده")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? SenderUserID { get; set; }

        [Display(Name = "سطح وظیفه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, int.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public int DutyPassLevel { get; set; }

        [Display(Name = "عنوان وظیفه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DutyTitle { get; set; }

        [Display(Name = "شرح وظیفه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DutyDescription { get; set; }

        [Display(Name = "خوانده شده")]
        public bool DutyIsRead { get; set; }

        [Display(Name = "انجام شده")]
        public bool DutyIsDone { get; set; }

        [Display(Name = "گزارش وظیفه")]
        public string? DutyReport { get; set; } = "";

        [Display(Name = "امتیاز وظیفه")]
        public float? DutyScore { get; set; } = 0.0f;

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
