using AITechDATA.Domain;

namespace AiTech.Domains
{
    public class PaymentInstallment : BaseEntity
    {
        public long PaymentHistoryId { get; set; }
        public PaymentHistory PaymentHistory { get; set; }

        public int InstallmentNumber { get; set; } // شماره قسط
        public decimal Amount { get; set; } // مبلغ قسط
        public DateTime DueDate { get; set; } // تاریخ سررسید
        public DateTime? PaidDate { get; set; } // تاریخ پرداخت
        public bool PayAllowed { get; set; } = false; // مجاز به پرداخت
        public bool IsPaid { get; set; } = false; // وضعیت پرداخت

    }
}