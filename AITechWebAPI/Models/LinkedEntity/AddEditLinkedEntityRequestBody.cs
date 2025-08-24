using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LinkedEntity
{
    public class AddEditLinkedEntityRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نوع ارتباط")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LinkType { get; set; }

        [Display(Name = "کلید خارجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "کد رکورد مرتبط")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long LinkedEntityId { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, int.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public int Priority { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityName { get; set; }

        [Display(Name = "عنوان")]
        public string? Title { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long? CreatorId { get; set; }
    }
}