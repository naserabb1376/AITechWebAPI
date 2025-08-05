using AITechWebAPI.Models.Address;
using AITechWebAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.User
{
    public class AddEditUserRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "آدرس")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? AdressId { get; set; }

        [Display(Name = "نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long RoleId { get; set; }

        [Display(Name = "نام کامل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FullName { get; set; }

        //[Display(Name = "نام خانوادگی")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //public string LastName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
        [Display(Name = "پست الکترونیک")]
        public string Email { get; set; }

        //[Display(Name = "شماره موبایل")]
        //[Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        //[RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        //[MaxLength(11)]
        //public string PhoneNumber { get; set; }

        [Display(Name = "کد ملی")]
        [ValidateOptional(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? NationalCode { get; set; }


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

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";
    }

    public class AddEditUserProRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام کامل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
        [Display(Name = "پست الکترونیک")]
        public string Email { get; set; }

        //[Display(Name = "شماره موبایل")]
        //[Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        //[RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        //[MaxLength(11)]
        //public string PhoneNumber { get; set; }

        //[Display(Name = "کد ملی")]
        //[RegularExpression(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        //[MaxLength(10)]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //public string NaCode { get; set; }
        //public string? DateOfBirth { get; set; }
        // public string? Description { get; set; }

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

        [Display(Name = "کد ملی")]
        [ValidateOptional(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? NationalCode { get; set; }

        public AddEditAddressRequestBody? Address { get; set; }

        [Display(Name = "نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long RoleId { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }

    public class EditUserRequestBody
    {
        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long ID { get; set; }

        [Display(Name = "آدرس")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? AdressId { get; set; }

        [Display(Name = "نقش")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? RoleId { get; set; }

        [Display(Name = "نام کامل")]
        public string? FullName { get; set; }

        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
        [Display(Name = "پست الکترونیک")]
        public string? Email { get; set; }

        [Display(Name = "کد ملی")]
        [ValidateOptional(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? NationalCode { get; set; }

        [Display(Name = "نام کاربری")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        public string? UserName { get; set; }

        [Display(Name = "رمز عبور")]
        [MaxLength(20)]
        [DataType(DataType.Password)] //Hide Characters
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "رمز عبور باید شامل حرف و عدد باشد")]
        public string? Password { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
