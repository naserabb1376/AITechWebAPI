using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.DiscountTarget
{
    public class AddEditDiscountTargetRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long DiscountId { get; set; }

        [Display(Name = "کد مشمول تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long TargetId { get; set; }

        [Display(Name = "موجودیت مشمول تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TargetEntityName { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; }

    }
}
