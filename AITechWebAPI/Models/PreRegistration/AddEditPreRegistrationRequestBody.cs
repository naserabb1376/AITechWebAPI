using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PreRegistration
{
    public class AddEditPreRegistrationRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام دانشجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FullName { get; set; } // عنوان دانشجو

        [Display(Name = "ایمیل دانشجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]

        public string Email { get; set; } // 

        [Display(Name = "شماره تماس دانشجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]

        public string PhoneNumber { get; set; }

        [Display(Name = "کد گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public string? RegistrationDate { get; set; } // تاریخ ثبت نام

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";



    }
}
