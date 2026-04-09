

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    // Duty: جدول وظیفه‌ها
    public class DutyVM : BaseVM
    {
        public string DutyTitle { get; set; }
        public string DutyDescription { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public long? SenderUserId { get; set; } // کلید خارجی به User
        public string? SenderUserName { get; set; } // ارتباط با User
        public int DutyPassLevel { get; set; }
        public bool IsRead { get; set; }
        public bool IsDone { get; set; }
        public string? DutyReport { get; set; }
        public float DutyScore { get; set; }
    }
}