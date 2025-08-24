

using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Notification: جدول اعلان‌ها
    public class NotificationVM : BaseVM
    {
        public string Message { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public bool IsRead { get; set; }
    }
}