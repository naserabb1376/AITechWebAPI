using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ // Permission: جدول دسترسی‌ها
    public class PermissionVM : BaseVM
    {
        public string Name { get; set; } // نام دسترسی (مثلاً Create, Edit, Delete)
        public string Name_EN { get; set; }
        public string Icon { get; set; }
        public string Routename { get; set; }
        public string PermissionType { get; set; }
        public string Description { get; set; } // توضیحات دسترسی
        public string Description_EN { get; set; } // توضیحات دسترسی
    }
}