

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    // Dismissal: جدول مرخصی‌ها
    public class DismissalVM : BaseVM
    {
        public string? DismissalRequestDescription { get; set; }
        public string DismissalType { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public long? CheckerUserId { get; set; } // کلید خارجی به User
        public string? CheckerUserName { get; set; } // ارتباط با User
        public string? CheckerDescription { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DismissalRequestStartDate { get; set; }
        public DateTime DismissalRequestEndDate { get; set; }
        public DateTime? DismissalApprovedStartDate { get; set; }
        public DateTime? DismissalApprovedEndDate { get; set; }

    }
}