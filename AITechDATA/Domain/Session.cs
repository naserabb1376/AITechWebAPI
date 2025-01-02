using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Session: جدول جلسات دوره‌ها
    public class Session : BaseEntity
    {
        public long GroupId { get; set; } // کلید خارجی به Group
        public Group Group { get; set; } // ارتباط با Group
        public DateTime SessionDate { get; set; } // تاریخ برگزاری جلسه
        public ICollection<Attendance> Attendances { get; set; } // حضور و غیاب مرتبط با جلسه
        public ICollection<SessionAssignment> SessionAssignments { get; set; } // تمرین‌های مرتبط با جلسه
    }
}