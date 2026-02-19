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
        public string? Note { get; set; }
        public DateTime EventDate { get; set; } // تاریخ رویداد
        public string Keywords { get; set; } // کلمات کلیدی برای سئو
        public decimal? Fee { get; set; } // هزینه رویداد

        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
    }

    public class EventDto : BaseEntity
    {
        public string Title { get; set; } // عنوان رویداد
        public string Description { get; set; } // توضیحات رویداد
        public string? Note { get; set; }
        public DateTime EventDate { get; set; } // تاریخ رویداد
        public string Keywords { get; set; } // کلمات کلیدی برای سئو
        public decimal? Fee { get; set; } // هزینه رویداد
        public int DiscountPercent { get; set; }
        public decimal? DiscountedFee { get; set; }

        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
    }

}