using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    // Minutes: جدول صورتجلسه کاری
    public class Minutes : BaseEntity
    {
        public string MinutesSubject { get; set; }
        public string MinutesDescription { get; set; }
        public long MeetingId { get; set; }

        public Meeting Meeting { get; set; }
    }
}