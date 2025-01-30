using AITechWebAPI.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class AuthenticationRequestBody
    {
        [Display(Name = "نام کاربری")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        [MaxLength(20)]
        [ConditionalRegularExpression("LoginType", @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "رمز عبور باید شامل حرف و عدد باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Password { get; set; }
        public int LoginType { get; set; } = 1;
    }
}
