

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    // Notification: جدول اعلان‌ها
    public class NotificationVM : BaseVM
    {
        public string Message { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public long? SenderUserId { get; set; } // کلید خارجی به User
        public string? SenderUserName { get; set; } // ارتباط با User
        public int NotificationPassLevel { get; set; }
        public bool IsRead { get; set; }
        public string? NotificationResponse { get; set; }
    }
}