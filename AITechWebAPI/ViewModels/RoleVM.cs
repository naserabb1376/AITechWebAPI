using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Role: جدول نقش‌ها
    public class RoleVM : BaseVM
    {
        public string Name { get; set; } // نام نقش (مثلاً Student, Teacher, Admin)
        public string? Description { get; set; } // توضیحات نقش
     
    }
}