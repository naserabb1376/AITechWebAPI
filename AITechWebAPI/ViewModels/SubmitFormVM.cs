using AITechDATA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class SubmitFormVM : BaseVM
    {
        public string FormKey { get; set; }
        public string EntityName { get; set; } // نام جدول مرتبط (مثلاً "User", "Course", "Event")
        public string? Title { get; set; }
        public string? Description { get; set; } = ""; // توضیحات
        public string? FormConfig { get; set; } = ""; // تنظیمات ساخت و نمایش فرم
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده

    }
}
