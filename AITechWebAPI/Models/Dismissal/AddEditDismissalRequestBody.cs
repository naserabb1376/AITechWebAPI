using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Dismissal
{
    public class AddEditDismissalRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "کد کاربر بررسی کننده")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? CheckerUserID { get; set; }

        [Display(Name = "نوع مرخصی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DismissalType { get; set; }

        [Display(Name = "شرح درخواست مرخصی")]
        public string? DismissalRequestDescription { get; set; }

        [Display(Name = "تایید شده")]
        public bool DismissalApproved { get; set; }

        [Display(Name = "شرح بررسی مرخصی")]
        public string? DismissalCheckDescription { get; set; }

        [Display(Name = "تاریخ درخواستی شروع مرخصی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DismissalRequestStartDate { get; set; }

        [Display(Name = "تاریخ درخواستی پایان مرخصی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string DismissalRequestEndDate { get; set; }

        [Display(Name = "تاریخ تایید شده شروع مرخصی")]
        public string? DismissalApprovedStartDate { get; set; }

        [Display(Name = "تاریخ تایید شده پایان مرخصی")]
        public string? DismissalApprovedEndDate { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
