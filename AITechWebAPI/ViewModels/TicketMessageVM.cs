using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // TicketMessage: جدول پیام‌های مرتبط با تیکت
    public class TicketMessageVM : BaseEntity
    {
        public long TicketId { get; set; } // کلید خارجی به Ticket
        public long? AdminId { get; set; } // کلید خارجی به User (ادمینی که پاسخ داده است)
        public string AdminUserName { get; set; } // ارتباط با User (ادمین)
        public string MessageContent { get; set; } // متن پیام
        public bool IsAdminResponse { get; set; } // مشخص می‌کند پیام از سمت ادمین است یا کاربر
    }
}