using Microsoft.AspNetCore.Http.Connections;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.EntityScore
{
    public class AddEditEntityScoreRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان")]
        public string? ScoreItemTitle { get; set; }

        [Display(Name = "کلید شاخص")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ScoreItemKey { get; set; }

        [Display(Name = "ضریب شاخص")]
        public float? ScoreItemWeight { get; set; } = 0.0f;

        //[Display(Name = "امتیاز کل")]
        //public float? ScoreItemTotalScore { get; set; } = 0.0f;

        [Display(Name = "کد والد")]
        public long? ParentId { get; set; }

        [Display(Name = "سطح رکورد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(0, 2, ErrorMessage = "مقدار {0} باید بین 0 تا 2 باشد")]
        public int RecordLevel { get; set; }

        [Display(Name = "کد رکورد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityType { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "نام موجودیت")]
        public string? TargetObjName { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long? CreatorId { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

    }
}
