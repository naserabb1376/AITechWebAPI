using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // AdminReport: جدول گزارش‌های پرسنل به ادمین اصلی
    public class AdminReportVM : BaseVM
    {
        public string Title { get; set; } // عنوان گزارش
        public string Content { get; set; } // محتوای گزارش
        public long AdminId { get; set; } // کلید خارجی به User (ادمینی که گزارش را ارسال کرده است)
        public string AdminUserName { get; set; } // ارتباط با User
        public float ReportScore { get; set; }
        public DateTime ReportDate { get; set; } // تاریخ گزارش
    }
}