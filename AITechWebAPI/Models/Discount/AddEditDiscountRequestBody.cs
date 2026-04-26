using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Discount
{
    public class AddEditDiscountRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد تخفیف")]
        public string? DiscountCode { get; set; }

        [Display(Name = "مقدار تخفیف")]
        public string DiscountValue { get; set; } = "0";

        [Display(Name = "کلید خارجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "موجودیت تخفیف دار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityName { get; set; }

        [Display(Name = "سقف استفاده")]
        public int DiscountMaxUsage { get; set; } = 0;

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "تاریخ انقضاء تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ExpireDate { get; set; }

        [Display(Name = "اعمال با کد تخفیف")]
        public bool CodeRequired { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long? CreatorId { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; }
    }
}