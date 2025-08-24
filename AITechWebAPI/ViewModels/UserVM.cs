using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // User: جدول کاربران
    public class UserVM : BaseVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string NationalCode { get; set; }
        public string Username { get; set; }
        public long RoleId { get; set; } // کلید خارجی به Role
        public string RoleName { get; set; } // ارتباط با Role
        public long? AddressId { get; set; } // کلید خارجی به Address    }
    }

    public class TeacherVM : BaseVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}