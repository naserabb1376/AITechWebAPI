using Microsoft.AspNetCore.Http.Connections;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentInstallment
{
    public class AddEditPaymentInstallmentRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long PaymentHistoryID { get; set; }

        [Display(Name = "شماره قسط")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, 10, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public int InstallmentNumber { get; set; }

        [Display(Name = "مبلغ پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public decimal Amount { get; set; }

        [Display(Name = "تاریخ پرداخت")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? PaidDate { get; set; }

        [Display(Name = "تاریخ سررسید")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? DueDate { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        public bool IsPaid { get; set; } // وضعیت پرداخت

        [Display(Name = "مجاز به پرداخت")]
        public bool PayAllowed { get; set; } // مجاز به پرداخت

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

    }
}
