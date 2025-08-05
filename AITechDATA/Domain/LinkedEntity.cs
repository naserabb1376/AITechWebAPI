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
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public long LinkedEntityId { get; set; } // کلید خارجی به رکورد مرتبط
        public int Priority { get; set; } // اولویت
        public string EntityName { get; set; } // نام جدول مرتبط (مثلاً "User", "Course", "Event")
        public string Description { get; set; } = ""; // توضیحات تصویر
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده
    }
}