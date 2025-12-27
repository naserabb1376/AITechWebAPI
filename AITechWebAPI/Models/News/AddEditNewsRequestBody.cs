using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.News
{
    public class AddEditNewsRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان خبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } // عنوان خبر

        [Display(Name = "محنوای خبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Content { get; set; } // محتوای خبر

        [Display(Name = "منبع خبر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Source { get; set; } // منبع خبر

        [Display(Name = "کلمات کلیدی سئو")]
        public string? Keywords { get; set; } = "";

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        [Display(Name = "تاریخ انتشار خبر")]
        public string? PublishDate { get; set; } // تاریخ انتشار خبر

        [Display(Name = "توضیحات")]
        public string? Note { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; }

    }
}
