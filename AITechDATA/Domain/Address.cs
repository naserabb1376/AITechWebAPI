using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{    // Address: جدول آدرس‌ها
    public class Address : BaseEntity
    {
        public string Street { get; set; } // خیابان
        public string City { get; set; } // شهر
        public string State { get; set; } // استان
        public string PostalCode { get; set; } // کد پستی
    }
}