using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Dismissal: جدول مرخصی‌ها
    public class Dismissal : BaseEntity
    {
        public string? DismissalRequestDescription { get; set; }
        public string DismissalType { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long? CheckerUserId { get; set; } // کلید خارجی به User

        [ForeignKey("CheckerUserId")]
        public User? CheckerUser { get; set; } // ارتباط با User
        public string? CheckerDescription { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DismissalRequestStartDate { get; set; }
        public DateTime DismissalRequestEndDate { get; set; }
        public DateTime? DismissalApprovedStartDate { get; set; }
        public DateTime? DismissalApprovedEndDate { get; set; }

    }

 
}