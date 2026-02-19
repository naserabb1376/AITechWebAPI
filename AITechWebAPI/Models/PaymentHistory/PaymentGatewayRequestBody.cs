using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentHistory
{
    public class RequestPaymentRequestBody
    {
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
        public long UserId { get; set; }

        [Display(Name = "شناسه تخفیف")]
        public long? DiscountId { get; set; }

    }

    public class VerifyPaymentRequestBody
    {
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
        public long UserId { get; set; }

        [Display(Name = "کد پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long PayID { get; set; }

        [Display(Name = "توکن پرداخت")]
        public string? PaymentToken { get; set; }

        [Display(Name = "شناسه پرداخت")]
        public string? PaymentAuthority { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        public string? PaymentStatus { get; set; }


    }

    public class RequestPaymentResultBody
    {
        [Display(Name = "آدرس درگاه پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PayGatewayUrl { get; set; }

    }
}
