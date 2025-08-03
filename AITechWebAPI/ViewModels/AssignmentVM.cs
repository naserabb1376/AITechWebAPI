using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Assignment: جدول ارسال تمرین‌ها توسط دانش‌آموزان
    public class AssignmentVM : BaseEntity
    {
        public string Title { get; set; } // عنوان تمرین
        public string Description { get; set; } // توضیحات تمرین
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } 
        public long SessionAssignmentId { get; set; } // کلید خارجی به SessionAssignment
        public DateTime SubmissionDate { get; set; } // تاریخ ارسال تمرین
    }
}