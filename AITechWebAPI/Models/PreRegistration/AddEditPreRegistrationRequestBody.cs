using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PreRegistration
{
    public class AddEditPreRegistrationRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LastName { get; set; }

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

        [Display(Name = "کلید خارجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityType { get; set; }

        [Display(Name = "نام موجودیت")]
        public string? TargetObjName { get; set; }

        [Display(Name = "پایه تحصیلی")]
        public string? EducationalClass { get; set; } // پایه تحصیلی

        [Display(Name = "نام مدرسه")]
        public string? SchoolName { get; set; }

        [Display(Name = "حوزه علاقمندی")]
        public string? FavoriteField { get; set; } // حوزه علاقمندی

        [Display(Name = "میزان آشنایی با حوزه علاقمندی")]
        public string? RecognitionLevel { get; set; } // میزان آشنایی با حوزه علاقمندی
      
        [Display(Name = "میزان تسلط به زبان های برنامه نویسی")]
        public string? ProgrammingSkillLevel { get; set; } // میزان تسلط به زبان های برنامه نویسی

        [Display(Name = " آدرس شبکه اجتماعی")]
        public string? SocialAddress { get; set; } // آدرس شبکه اجتماعی


        [Display(Name = "تاریخ ثبت نام")]
        public string? RegistrationDate { get; set; } // تاریخ ثبت نام

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";



    }
}
