using AITechDATA.Domain;

namespace AITechWebAPI.ViewModels
{
    // News: جدول اخبار دنیای هوش مصنوعی و سایر موضوعات
    public class NewsVM : BaseEntity
    {
        public string Title { get; set; } // عنوان خبر
        public string Content { get; set; } // محتوای خبر
        public string Source { get; set; } // منبع خبر
        public DateTime PublishDate { get; set; } // تاریخ انتشار خبر
        public string Keywords { get; set; } // کلمات کلیدی برای سئو
        public string? Note { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public string UserName { get; set; } // ارتباط با User
    }
}