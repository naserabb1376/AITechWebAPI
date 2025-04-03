using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // FileUpload: جدول فایل‌های ارسال‌شده
    public class FileUpload : BaseEntity
    {
        public string FileName { get; set; } // نام فایل
        public string FilePath { get; set; } // مسیر فایل
        public string ContentType { get; set; } // نوع فایل (مثلاً PDF, JPEG)
        public long AssignmentId { get; set; } // کلید خارجی به Assignment
        public string Description { get; set; } = ""; // توضیحات فایل
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده

        public Assignment Assignment { get; set; } // ارتباط با Assignment
    }
}