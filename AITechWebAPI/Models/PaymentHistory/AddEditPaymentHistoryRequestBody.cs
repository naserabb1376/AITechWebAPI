using Microsoft.AspNetCore.Http.Connections;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentHistory
{
    public class AddEditPaymentHistoryRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد رکورد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long ForeignKeyId { get; set; }

        [Display(Name = "نام جدول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityType { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "مبلغ پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public decimal Amount { get; set; }

        [Display(Name = "تاریخ پرداخت")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? PaymentDate { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        public bool PaymentStatus { get; set; } // وضعیت پرداخت

    }
}
