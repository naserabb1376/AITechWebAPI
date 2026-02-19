using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class DiscountTarget : BaseEntity
    {
        public long TargetId { get; set; }
        public long DiscountId { get; set; }
        public string TargetEntityName { get; set; } // نام جدول مقصد (مثلاً "User", "Course", "Event")
        public Discount Discount { get; set; } = default!;
    }
}