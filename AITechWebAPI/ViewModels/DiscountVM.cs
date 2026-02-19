using AITechDATA.Domain;
using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class DiscountVM : BaseVM
    {
        public string DiscountCode { get; set; } 
        public int DiscountPercent { get; set; }
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityName { get; set; } // نام جدول مبدا (مثلاً "User", "Course", "Event")
        public string TargetEntityName { get; set; } // نام جدول مقصد (مثلاً "User", "Course", "Event")
        public bool CodeRequired { get; set; }
        public string? Description { get; set; } = ""; // توضیحات
        public long CreatorId { get; set; } = 0; // کاربر ایجاد کننده
    }
}