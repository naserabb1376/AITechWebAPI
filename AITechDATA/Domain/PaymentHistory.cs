using AITechDATA.Domain;

namespace AiTech.Domains
{
    // PaymentHistory: جدول تاریخچه پرداخت‌ها
    public class PaymentHistory : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long GroupId { get; set; } // کلید خارجی به Group
        public Group Group { get; set; } // ارتباط با Group
        public bool PaymentStatus { get; set; } // وضعیت پرداخت
    }
}