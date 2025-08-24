using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // TeacherResume: جدول رزومه اساتید
    public class TeacherResumeVM : BaseVM
    {
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
        public string Title { get; set; } // عنوان رزومه (مثلاً مدرک یا پروژه)
        public string Description { get; set; } // توضیحات رزومه
        public DateTime DateAchieved { get; set; } // تاریخ کسب یا انجام
    }
}