using AITechDATA.Domain;
using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class DiscountTargetVM : BaseVM
    {
        public long TargetId { get; set; } 
        public string DiscountCode { get; set; } 
        public int DiscountPercent { get; set; }
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityName { get; set; } // نام جدول مبدا (مثلاً "User", "Course", "Event")
        public string TargetEntityName { get; set; } // نام جدول مقصد (مثلاً "User", "Course", "Event")
        public string? Description { get; set; } = ""; // توضیحات
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده
        public bool CodeRequired { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}