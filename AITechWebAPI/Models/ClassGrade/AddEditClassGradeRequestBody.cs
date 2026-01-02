using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.ClassGrade
{
    public class AddEditClassGradeRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کلید خارجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityName { get; set; }

        [Display(Name = "نمره کلاسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, float.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public float GradeScore { get; set; } // نمره کلاسی

        [Display(Name = "عنوان")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? Title { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
}