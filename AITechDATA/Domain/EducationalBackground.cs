using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // EducationalBackground: جدول پیشینه تحصیلی
    public class EducationalBackground : BaseEntity
    {
        public long UserId { get; set; } // کلید خارجی به User
        public string StudyField { get; set; } // رشته تحصیلی
        public string EducationalGrade { get; set; } // مقطع تحصیلی
        public string? Description { get; set; } //
        public User User { get; set; } // ارتباط با User
    }
}