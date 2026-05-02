using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Discount
{
    public class ExistDiscountRequestBody
    {
        [Display(Name = "نوع جستجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string ExistType { get; set; }

        [Display(Name = "مقدار کلید جستجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string KeyValue { get; set; }

        [Display(Name = "نام موجودیت")]
        public string? EntityType { get; set; }

        [Display(Name = "کد رکورد موجودیت")]
        public long? ForeignkeyId { get; set; }

    }
}