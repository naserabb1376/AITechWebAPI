using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // SessionAssignment: جدول تمرین‌های مرتبط با جلسات
    public class SessionAssignmentVM : BaseVM
    {
        public long SessionId { get; set; } // کلید خارجی به Session
        public string Title { get; set; } // عنوان تمرین
        public string Description { get; set; } // توضیحات تمرین
        public DateTime DueDate { get; set; } // مهلت ارسال تمرین
    }
}