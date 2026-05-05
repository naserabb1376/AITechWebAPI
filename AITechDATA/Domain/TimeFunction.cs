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

    public class TimeFunctionDto : BaseEntity
    {

        public DateTime TimeFunctionStartDate { get; set; }
        public DateTime TimeFunctionEndDate { get; set; }

        // این دوتا برای کل بازه تاریخی فیلتر شده هستند
        public TimeSpan? TotalRangeBreakTime { get; set; }
        public TimeSpan? TotalRangeUsefulWorkTime { get; set; }

        public long UserId { get; set; }
        public string? UserFullName { get; set; }

        public string? Description { get; set; }

        public List<TimeBreak> TimeBreaks { get; set; }

        public TimeSpan TotalBreakTime { get; set; }

        public TimeSpan TotalUsefulWorkTime { get; set; }

        public string TotalBreakTimeText => FormatTimeSpan(TotalBreakTime);

        public string TotalUsefulWorkTimeText => FormatTimeSpan(TotalUsefulWorkTime);

        private static string FormatTimeSpan(TimeSpan time)
        {
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }
}