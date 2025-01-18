using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{  // Category: جدول دسته‌بندی‌ها
    public class Category : BaseEntity
    {
        [Display(Name = "نام دسته بندی")]
        public string CategoryName { get; set; }

        [Display(Name = "توضیحات")]
        public string? CategoryDescription { get; set; }

        public ICollection<Course> Courses { get; set; } // ارتباط یک به چند با Course
    }
}