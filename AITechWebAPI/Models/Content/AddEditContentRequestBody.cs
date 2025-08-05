using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Content
{
    public class AddEditContentRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کلید خارجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityType { get; set; }

        [Display(Name = "متن محتوا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "اولویت")]
        public int? Priority { get; set; }

        [Display(Name = "دارای تصویر")]
        public bool HaveImage { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}