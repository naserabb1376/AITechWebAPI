

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    public class TimeFunctionVM : BaseVM
    {
        public DateTime TimeFunctionStartDate { get; set; }
        public DateTime TimeFunctionEndDate { get; set; }

        // این دوتا برای کل بازه تاریخی فیلتر شده هستند
        public TimeSpan? TotalRangeBreakTime { get; set; }
        public TimeSpan? TotalRangeUsefulWorkTime { get; set; }

        public long UserId { get; set; }
        public string? UserFullName { get; set; }

        public string? Description { get; set; }

        public TimeSpan TotalBreakTime { get; set; }

        public TimeSpan TotalUsefulWorkTime { get; set; }

        public string TotalBreakTimeText { get; set; }

        public string TotalUsefulWorkTimeText { get; set; }

    }
}