using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class SignupRequestBody
    {
        [Display(Name = "نام کاربری")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        [MaxLength(20)]
        [DataType(DataType.Password)] //Hide Characters
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "رمز عبور باید شامل حرف و عدد باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Password { get; set; }

        [Display(Name = "نقش کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long RoleId { get; set; } = 1; // 1: Student 2: Teacher

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
        [Display(Name = "پست الکترونیک")]
        public string Email { get; set; }

        [Display(Name = "شهر یا استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long CityID { get; set; }

        [Display(Name = "خیابان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AddressStreet { get; set; }

        [Display(Name = "کد پستی")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AddressPostalCode { get; set; }
        public string? AddressLocationHorizentalPoint { get; set; }
        public string? AddressLocationVerticalPoint { get; set; }
    }
}