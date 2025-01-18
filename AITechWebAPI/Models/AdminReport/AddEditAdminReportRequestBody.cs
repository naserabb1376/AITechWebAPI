using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.AdminReport
{
    public class AddEditAdminReportRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان گزارش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } // عنوان گزارش

        [Display(Name = "محتوای گزارش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Content { get; set; } // محتوای گزارش

        [Display(Name = "کد کاربری مدیر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long AdminId { get; set; } // کلید خارجی به User (ادمینی که گزارش را ارسال کرده است)

        [Display(Name = "تاریخ گزارش")]
        public string? ReportDate { get; set; } // تاریخ گزارش
    }
}
