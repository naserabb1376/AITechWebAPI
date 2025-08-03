using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // StudentDetails: جدول جزئیات دانش‌آموزان
    public class StudentDetailsVM : BaseEntity
    {
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } 
    }
}