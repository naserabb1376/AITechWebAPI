using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.FormField
{
    public class AddEditFormFieldRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام فیلد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FieldName { get; set; }

        [Display(Name = "عنوان فیلد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DisplayName { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; }

    }
}