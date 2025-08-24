using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // Event: جدول اخبار و رویدادها
    public class EventVM : BaseVM
    {
        public string Title { get; set; } // عنوان رویداد
        public string Description { get; set; } // توضیحات رویداد
        public string? Note { get; set; }
        public DateTime EventDate { get; set; } // تاریخ رویداد
        public string Keywords { get; set; } // کلمات کلیدی برای سئو

        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
    }
}