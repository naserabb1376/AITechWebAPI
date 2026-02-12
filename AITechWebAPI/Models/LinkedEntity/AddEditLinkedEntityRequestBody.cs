using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LinkedEntity
{
    public class AddEditLinkedEntityRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نوع ارتباط")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LinkType { get; set; }

        [Display(Name = "کد رکورد مبدا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long SourceRowId { get; set; }

        [Display(Name = "کد رکورد مقصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long DestRowId { get; set; }

        [Display(Name = "نام جدول مبدا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SourceTableName { get; set; }

        [Display(Name = "نام جدول مقصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DestTableName { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long? CreatorId { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; }
    }
}