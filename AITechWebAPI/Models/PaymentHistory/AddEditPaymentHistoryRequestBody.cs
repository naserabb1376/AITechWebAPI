using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentHistory
{
    public class AddEditPaymentHistoryRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupID { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "مبلغ پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public decimal Amount { get; set; }

        [Display(Name = "تاریخ پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? PaymentDate { get; set; }
    }
}
