using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // PaymentHistory: جدول تاریخچه پرداخت‌ها
    public class PaymentHistoryVM : BaseVM
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? TargetObjName { get; set; }
        public bool PaymentStatus { get; set; } // وضعیت پرداخت
        public long? DiscountId { get; set; }
    }
}