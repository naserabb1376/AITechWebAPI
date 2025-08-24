using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Ticket: جدول تیکت‌ها
    public class TicketVM : BaseVM
    {
        public string Subject { get; set; } // موضوع تیکت
        public string Description { get; set; } // توضیحات تیکت
        public long UserId { get; set; } // کلید خارجی به User (کاربری که تیکت را ثبت کرده است)
        public string UserName { get; set; } // ارتباط با User
    }
}