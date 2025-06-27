using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Group
{
    public class AddEditGroupRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; } 

        [Display(Name = "روز هفته")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DayOfWeek { get; set; }

        [Display(Name = "تاریخ شروع")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string StartDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EndDate { get; set; }

        [Display(Name = "ساعت شروع")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string StartTime { get; set; }

        [Display(Name = "ساعت پایان")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? EndTime { get; set; }

        [Display(Name = "وضعیت گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string GroupStatus { get; set; }

        [Display(Name = "هزینه گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public decimal GroupFee { get; set; }

        [Display(Name = "کد مدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long TeacherId { get; set; }

        [Display(Name = "کد درس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long CourseId { get; set; }

        [Display(Name = "توضیحات")]
        public string? Note { get; set; }

    }
}
