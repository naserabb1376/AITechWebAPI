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
        public string CategoryName { get; set; }
        public string CategoryEntityType { get; set; }
        public string? CategoryDescription { get; set; }

        public ICollection<Course> Courses { get; set; } // ارتباط یک به چند با Course
        public ICollection<Article>? Articles { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}