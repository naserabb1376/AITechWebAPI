using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Attendance: جدول حضور و غیاب
    public class Attendance : BaseEntity
    {
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long SessionId { get; set; } // کلید خارجی به Session
        public Session Session { get; set; } // ارتباط با Session
        public bool IsPresent { get; set; }
    }
}