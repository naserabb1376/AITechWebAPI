using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{ // Permission: جدول دسترسی‌ها
    public class Permission : BaseEntity
    {
        public string Name { get; set; } // نام دسترسی (مثلاً Create, Edit, Delete)
        public string Description { get; set; } // توضیحات دسترسی
        public ICollection<Role> Roles { get; set; } // نقش‌های مرتبط با دسترسی
    }
}