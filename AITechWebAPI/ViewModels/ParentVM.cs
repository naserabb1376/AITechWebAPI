using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Parent: جدول والدین
    public class ParentVM : BaseEntity
    {
        public string Name { get; set; } // نام والد
        public string Job { get; set; } // شغل والد
        public string ContactNumber { get; set; } // شماره تماس والد
        public long StudentDetailsId { get; set; } // کلید خارجی به StudentDetails
        public string StudentName { get; set; } // ارتباط با StudentDetails
    }
}