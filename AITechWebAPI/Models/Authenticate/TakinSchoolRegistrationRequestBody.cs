using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class TakinSchoolRegistrationRequestBody
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LastName { get; set; }

        [Display(Name = "کد ملی")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string NationalCode { get; set; }

        [Display(Name = "شماره موبایل ورود")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PhoneNumber { get; set; }

        [Display(Name = "رمز عبور")]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@#!_\-]{6,20}$", ErrorMessage = "رمز عبور باید شامل حرف و عدد باشد")]
        public string? Password { get; set; }

        [Display(Name = "پست الکترونیک")]
        [EmailAddress(ErrorMessage = "پست الکترونیک معتبر نیست")]
        public string? Email { get; set; }

        [Display(Name = "نام دبستان محل تحصیل سال جاری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CurrentSchoolName { get; set; }

        [Display(Name = "پایه ثبت نامی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string TargetGrade { get; set; }

        [Display(Name = "آدرس محل زندگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AddressStreet { get; set; }

        [Display(Name = "کد پستی")]
        [RegularExpression(@"^$|^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        public string? AddressPostalCode { get; set; }

        [Display(Name = "شهر")]
        public long CityID { get; set; } = 0;

        [Display(Name = "نام پدر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FatherName { get; set; }

        [Display(Name = "شماره پدر")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FatherPhone { get; set; }

        [Display(Name = "شغل پدر")]
        public string? FatherJob { get; set; }

        [Display(Name = "تحصیلات پدر")]
        public string? FatherEducation { get; set; }

        [Display(Name = "نام مادر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MotherName { get; set; }

        [Display(Name = "شماره مادر")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MotherPhone { get; set; }

        [Display(Name = "شغل مادر")]
        public string? MotherJob { get; set; }

        [Display(Name = "تحصیلات مادر")]
        public string? MotherEducation { get; set; }
    }
}
