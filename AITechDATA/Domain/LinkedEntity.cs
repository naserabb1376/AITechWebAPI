using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // LinkedEntity: جدول موجودیت های متبط
    public class LinkedEntity : BaseEntity
    {
        public string LinkType { get; set; } // نوع ارتباط
        public long SourceRowId { get; set; } // کلید خارجی به رکورد مبدا
        public long DestRowId { get; set; } // کلید خارجی به رکورد مقصد
        public string SourceTableName { get; set; } // نام جدول مبدا (مثلاً "User", "Course", "Event")
        public string DestTableName { get; set; } // نام جدول مقصد (مثلاً "User", "Course", "Event")
        public string? Description { get; set; } = ""; // توضیحات تصویر
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده
    }
}