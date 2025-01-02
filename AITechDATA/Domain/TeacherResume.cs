namespace AITechDATA.Domain
{
    // TeacherResume: جدول رزومه اساتید
    public class TeacherResume : BaseEntity
    {
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public string Title { get; set; } // عنوان رزومه (مثلاً مدرک یا پروژه)
        public string Description { get; set; } // توضیحات رزومه
        public DateTime DateAchieved { get; set; } // تاریخ کسب یا انجام
    }
}