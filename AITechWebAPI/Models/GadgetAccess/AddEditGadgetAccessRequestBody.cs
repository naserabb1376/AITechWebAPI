using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GadgetAccess
{
    public class AddEditGadgetAccessRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "شناسه گجت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string GadgetKey { get; set; }

        [Display(Name = "شرح گجت")]
        public string? GadgetDescription { get; set; }

        [Display(Name = "آدرس گجت")]
        public string? GadgetUrl { get; set; }

        [Display(Name = "نام کاربری دسترسی")]
        public string? AccessUserName { get; set; }

        [Display(Name = "رمز عبور دسترسی")]
        public string? AccessPassword { get; set; }

        [Display(Name = "تاریخ شروع دسترسی")]
        public string? AccessStartDate { get; set; }

        [Display(Name = "تاریخ پایان دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AccessEndDate { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
