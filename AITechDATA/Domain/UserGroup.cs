using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{ // Permission: جدول دسترسی‌ها
    public class UserGroup : BaseEntity
    {
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public long GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

    }
}