using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // ClassGrade: جدول نمرات کلاسی
    public class ClassGrade : BaseEntity
    {
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityName { get; set; } // نام جدول مرتبط (مثلاً "User", "Course", "Event")
        public float GradeScore { get; set; } // نمره کلاسی
        public string? Title { get; set; } 
        public string? Description { get; set; } = ""; // توضیحات
    }
}