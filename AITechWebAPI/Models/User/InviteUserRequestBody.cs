using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.User
{
    public class InviteUserRequestBody
    {
        [Display(Name = "شماره موبایل")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
    }
}