using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class TakinSchoolExamRegistrationLookupRequestBody
    {
        [Display(Name = "شماره موبایل")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PhoneNumber { get; set; }
    }
}
