

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    public class CommentVM : BaseVM
    {
        public string? Title { get; set; }
        public long ForeignKeyId { get; set; } // کلید خارجی به رکورد اصلی
        public string EntityType { get; set; } // نوع جدول مرتبط (مثلاً "User", "Course", "Event")
        public string Description { get; set; } = "";
        public long UserId { get; set; } = 0; // کاربر ایجاد کننده
        public string Username { get; set; } 
        public string FullName { get; set; } 
        public long ParentId { get; set; } = 0; // کد کامنت والد
    }
}