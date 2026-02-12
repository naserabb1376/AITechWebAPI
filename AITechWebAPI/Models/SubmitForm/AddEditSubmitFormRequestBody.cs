using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.SubmitForm
{
    public class AddEditSubmitFormRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کلید فرم")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FormKey { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityName { get; set; }

        [Display(Name = "عنوان فرم")]
        public string? FormTitle { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; }

        [Display(Name = "کد فیلدها")]
        public List<long> FieldIds { get; set; } = new List<long>();    


    }
}