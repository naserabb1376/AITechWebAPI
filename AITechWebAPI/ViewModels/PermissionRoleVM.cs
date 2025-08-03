using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ // Permission: جدول دسترسی‌ها
    public class PermissionRoleVM : BaseEntity
    {
        public long RoleId { get; set; }

        public string RoleName { get; set; }

        public long PerrmissionId { get; set; }

        public string PermissionName { get; set; }
    }
}