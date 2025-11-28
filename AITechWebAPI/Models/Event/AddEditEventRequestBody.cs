using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Event
{
    public class AddEditEventRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان رویداد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } // عنوان رویداد

        [Display(Name = "شرح رویداد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; } // شرح رویداد

        [Display(Name = "کلمات کلیدی سئو")]
        public string? Keywords { get; set; } = "";

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        [Display(Name = "تاریخ رویداد")]
        public string? EventDate { get; set; } // تاریخ رویداد


        [Display(Name = "هزینه رویداد")]
        public decimal? EventFee { get; set; }


        [Display(Name = "توضیحات")]
        public string? Note { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
