using AITechWebAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.JobRequest
{
    public class AddEditJobRequestRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LastName { get; set; }

        [Display(Name = "کد ملی")]
        [ValidateOptional(@"^([0-9]{10})$", ErrorMessage = "مقدار {0} باید 10 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(10)]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? NationalCode { get; set; }

        [Display(Name = "نام پدر")]
        // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? FatherName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
        [Display(Name = "پست الکترونیک")]
        public string Email { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Display(Name = "تاریخ تولد")]
        // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? BirthDate { get; set; }

        [Display(Name = "دانشگاه محل تحصیل")]
        // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? UniversityName { get; set; }

        [Display(Name = "آخرین مدرک تحصیلی")]
        // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? LastAcademicLicense { get; set; }

        [Display(Name = "مقطع تحصیلی")]
        // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? EducationalLevel { get; set; }

        [Display(Name = "وضعیت تحصیلی")]
        // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? EducationStatus { get; set; }

        [Display(Name = "شغل درخواستی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RequestedPosition { get; set; }

        [Display(Name = "عنوان دوره")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CourseTitle { get; set; }

        [Display(Name = "شرح درخواست")]
        public string? Description { get; set; } = "";

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
