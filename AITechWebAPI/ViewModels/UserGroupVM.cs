using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ // Permission: جدول دسترسی‌ها
    public class UserGroupVM : BaseVM
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        public long GroupId { get; set; }

        public string GroupName { get; set; }
        public string FullName { get; set; }

    }
}