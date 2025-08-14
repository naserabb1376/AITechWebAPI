using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // TicketMessage: جدول پیام‌های مرتبط با تیکت
    public class TicketMessageVM : BaseEntity
    {
        public long TicketId { get; set; } // کلید خارجی به Ticket
        public long? UserId { get; set; } // کلید خارجی به User 
        public string UserName { get; set; } // نام کاربری User
        public string MessageContent { get; set; } // متن پیام
        public bool IsAdminResponse { get; set; } // مشخص می‌کند پیام از سمت ادمین است یا کاربر
    }
}