using AITechDATA.Domain;

namespace AiTech.Domains
{
    // PaymentHistory: جدول تاریخچه پرداخت‌ها
    public class PaymentHistory : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public long? UserId { get; set; } // کلید خارجی به User
        public User? User { get; set; } // ارتباط با User
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? TargetObjName { get; set; }
        public bool PaymentStatus { get; set; } // وضعیت پرداخت
        public bool IsInstallment { get; set; } = false; // آیا این پرداخت قسطی است؟
        public ICollection<PaymentInstallment>? PaymentInstallments { get; set; }
        public long? PreRegistrationId { get; set; }
        public string? PaymentAuthority { get; set; } // کد Authority درگاه برای بررسی مجدد پرداخت
        public string? TransactionCode { get; set; } // شناسه ارجاع درگاه پرداخت

        public long? DiscountId { get; set; } 
        public Discount? Discount { get; set; }

    }
}
