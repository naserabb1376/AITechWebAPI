using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.FieldInForm
{
    public class AddEditFieldInFormRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد فرم")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long FormId { get; set; }

        [Display(Name = "کد فیلد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long FormFieldId { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; }
    }
}
