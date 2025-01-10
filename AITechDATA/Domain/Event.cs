using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Event: جدول اخبار و رویدادها
    public class Event : BaseEntity
    {
        public string Title { get; set; } // عنوان رویداد
        public string Description { get; set; } // توضیحات رویداد
        public DateTime EventDate { get; set; } // تاریخ رویداد
        public string Keywords { get; set; } // کلمات کلیدی برای سئو

        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
    }
}