using AiTech.Domains;
using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    public class PaymentInstallmentVM : BaseVM
    {
        public long PaymentHistoryId { get; set; }
        public int InstallmentNumber { get; set; } // شماره قسط
        public decimal Amount { get; set; } // مبلغ قسط
        public DateTime DueDate { get; set; } // تاریخ سررسید
        public DateTime? PaidDate { get; set; } // تاریخ پرداخت
        public bool IsPaid { get; set; } // وضعیت پرداخت
        public bool PayAllowed { get; set; } // مجاز به پرداخت

        public decimal PaymentHistoryAmount { get; set; }
        public DateTime PaymentHistoryDate { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? TargetObjName { get; set; }
        public bool PaymentHistoryStatus { get; set; } // وضعیت پرداخت
    }
}