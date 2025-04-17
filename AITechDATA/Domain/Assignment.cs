using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Assignment: جدول ارسال تمرین‌ها توسط دانش‌آموزان
    public class Assignment : BaseEntity
    {
        public string Title { get; set; } // عنوان تمرین
        public string Description { get; set; } // توضیحات تمرین
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long SessionAssignmentId { get; set; } // کلید خارجی به SessionAssignment
        public SessionAssignment SessionAssignment { get; set; } // ارتباط با SessionAssignment
        public DateTime SubmissionDate { get; set; } // تاریخ ارسال تمرین
        //public ICollection<FileUpload> Files { get; set; } // فایل‌های ارسال شده توسط دانش‌آموز
    }
}