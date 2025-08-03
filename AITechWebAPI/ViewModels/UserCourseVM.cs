using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{ // Permission: جدول دسترسی‌ها
    public class UserCourseVM : BaseEntity
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public int PeresentType { get; set; }

    }
}