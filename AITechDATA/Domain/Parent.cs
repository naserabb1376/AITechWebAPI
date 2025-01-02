using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Parent: جدول والدین
    public class Parent : BaseEntity
    {
        public string Name { get; set; } // نام والد
        public string Job { get; set; } // شغل والد
        public string ContactNumber { get; set; } // شماره تماس والد
        public long StudentDetailsId { get; set; } // کلید خارجی به StudentDetails
        public StudentDetails StudentDetails { get; set; } // ارتباط با StudentDetails
    }
}