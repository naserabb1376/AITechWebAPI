using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class Content : BaseEntity
    {
        public string? Description { get; set; } // توضیحات رویداد
        public int? Priority { get; set; }
        public bool HaveImage { get; set; }
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")
    }
}