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
        public long GroupId { get; set; } // کلید خارجی به Group
        public bool PaymentStatus { get; set; } // وضعیت پرداخت
    }
}