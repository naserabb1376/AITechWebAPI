using AiTech.Domains;
using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class Discount : BaseEntity
    {
        public string DiscountCode { get; set; } 
        public int DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public int DiscountMaxUsage { get; set; } = 0;
        public string EntityName { get; set; } // نام جدول مبدا (مثلاً "User", "Course", "Event")
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string? Description { get; set; } = ""; // توضیحات تصویر
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده
        public DateTime ExpireDate { get; set; }
        public bool CodeRequired { get; set; }

        public ICollection<DiscountTarget> DiscountTargets { get; set; } 
        public ICollection<PaymentHistory> PaymentHistories { get; set; } 

    }

    public class ShowDiscountDto
    {
        public int DiscountPercent { get; set; }
        public decimal DiscountedFee { get; set; }
        public decimal DiscountAmount { get; set; }

    }
}