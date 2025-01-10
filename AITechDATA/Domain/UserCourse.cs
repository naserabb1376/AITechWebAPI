using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{ // Permission: جدول دسترسی‌ها
    public class UserCourse : BaseEntity
    {
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public long CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public int PeresentType { get; set; }

    }
}