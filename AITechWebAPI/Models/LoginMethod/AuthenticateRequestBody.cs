using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LoginMethod
{
    public class AuthenticateRequestBody
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        //[MaxLength(11)]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(20)]
        //[DataType(DataType.Password)] //Hide Characters
        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "کلمه عبور باید شامل حرف و عدد باشد")] //check exist number & alphabet chars in password field
        public string Password { get; set; }
        public int AuthenticateType { get; set; } = 1;
    }
}
