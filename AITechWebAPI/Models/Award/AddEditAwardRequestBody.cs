using AITechWebAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Award
{
    public class AddEditAwardRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان جایزه")]
       // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? AwardTitle { get; set; }

        [Display(Name = "نام")]
       // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
      //  [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? LastName { get; set; }

        [MaxLength(200)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
        [Display(Name = "پست الکترونیک")]
        public string? Email { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Display(Name = "شرح درخواست")]
        public string? Description { get; set; } = "";

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
