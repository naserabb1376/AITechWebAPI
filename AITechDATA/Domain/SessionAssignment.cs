using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // SessionAssignment: جدول تمرین‌های مرتبط با جلسات
    public class SessionAssignment : BaseEntity
    {
        public long SessionId { get; set; } // کلید خارجی به Session
        public Session Session { get; set; } // ارتباط با Session
        public string Title { get; set; } // عنوان تمرین
        public string Description { get; set; } // توضیحات تمرین
        public DateTime DueDate { get; set; } // مهلت ارسال تمرین
        public ICollection<Assignment> Assignments { get; set; } // ارسال‌های انجام‌شده توسط دانش‌آموزان
    }
}