using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    public class TimeFunction : BaseEntity
    {
        public DateTime TimeFunctionStartDate { get; set; }
        public DateTime TimeFunctionEndDate { get; set; }
        public long UserId { get; set; } // کلید خارجی به User (کاربری که تیکت را ثبت کرده است)
        public User User { get; set; } // ارتباط با User
        public string? Description { get; set; }

        public List<TimeBreak> TimeBreaks { get; set; }
    }
}