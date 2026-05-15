using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserGroup
{
    public class AddEditUserGroupRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }

        [Display(Name = "کد گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long GroupId { get; set; }

        [Display(Name = "ثبت پرداخت دستی")]
        public bool RegisterManualPayment { get; set; } = false;

        [Display(Name = "مبلغ پرداخت دستی")]
        public decimal? ManualPaymentAmount { get; set; }

        [Display(Name = "تاریخ پرداخت دستی")]
        public string? ManualPaymentDate { get; set; }

        [Display(Name = "شناسه تخفیف")]
        public long? DiscountId { get; set; }

    }
}
