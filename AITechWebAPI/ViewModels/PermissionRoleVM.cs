using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ // Permission: جدول دسترسی‌ها
    public class PermissionRoleVM : BaseVM
    {
        public long RoleId { get; set; }
        public long PerrmissionId { get; set; }
        public string PermissionName { get; set; }
        public string RouteName { get; set; }
        public string PermissionType { get; set; }
        public bool OwnerOnly { get; set; }

    }
}