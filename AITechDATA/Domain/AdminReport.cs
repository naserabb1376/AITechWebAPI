using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // AdminReport: جدول گزارش‌های پرسنل به ادمین اصلی
    public class AdminReport : BaseEntity
    {
        public string Title { get; set; } // عنوان گزارش
        public string Content { get; set; } // محتوای گزارش
        public long AdminId { get; set; } // کلید خارجی به User (ادمینی که گزارش را ارسال کرده است)
        public User Admin { get; set; } // ارتباط با User
        public float ReportScore { get; set; }
        public DateTime ReportDate { get; set; } // تاریخ گزارش
    }
}