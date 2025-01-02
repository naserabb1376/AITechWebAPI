using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // TicketMessage: جدول پیام‌های مرتبط با تیکت
    public class TicketMessage : BaseEntity
    {
        public long TicketId { get; set; } // کلید خارجی به Ticket
        public Ticket Ticket { get; set; } // ارتباط با Ticket
        public long? AdminId { get; set; } // کلید خارجی به User (ادمینی که پاسخ داده است)
        public User Admin { get; set; } // ارتباط با User (ادمین)
        public string MessageContent { get; set; } // متن پیام
        public bool IsAdminResponse { get; set; } // مشخص می‌کند پیام از سمت ادمین است یا کاربر
    }
}