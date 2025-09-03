using AITechWebAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.JobRequest
{
    public class AddEditJobRequestRequestBody
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

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }


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
