using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // StudentDetails: جدول جزئیات دانش‌آموزان
    public class StudentDetails : BaseEntity
    {
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long AddressId { get; set; } // کلید خارجی به Address
        public Address Address { get; set; } // ارتباط با Address
        public ICollection<Parent> Parents { get; set; } // والدین دانش‌آموز
    }
}