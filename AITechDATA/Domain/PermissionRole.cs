using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{ // Permission: جدول دسترسی‌ها
    public class PermissionRole : BaseEntity
    {
        public long RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public long PerrmissionId { get; set; }

        [ForeignKey("RoleId")]
        public Permission Permission { get; set; }
    }
}