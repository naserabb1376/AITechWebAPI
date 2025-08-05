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
        public string Name_EN { get; set; }
        public string Icon { get; set; }
        public string Routename { get; set; }
        public string Description { get; set; } // توضیحات دسترسی
        public string Description_EN { get; set; } // توضیحات دسترسی
        public string? PermissionType { get; set; } // نوع دسترسی
        public ICollection<PermissionRole> PermissionRoles { get; set; } // دسترسی‌های مرتبط با نقش

        //public ICollection<Role> Roles { get; set; } // نقش‌های مرتبط با دسترسی
    }
}