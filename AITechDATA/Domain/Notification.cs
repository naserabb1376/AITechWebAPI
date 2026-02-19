using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Notification: جدول اعلان‌ها
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long? SenderUserId { get; set; } // کلید خارجی به User

        [ForeignKey("SenderUserId")]
        public User? SenderUser { get; set; } // ارتباط با User
        public int NotificationPassLevel { get; set; }
        public bool IsRead { get; set; }
    }
}