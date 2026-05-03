

using AITechDATA.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITechWebAPI.ViewModels
{
    public class TimeBreakVM : BaseVM
    {
        public TimeSpan TimeBreakStartTime { get; set; }
        public TimeSpan TimeBreakEndTime { get; set; }
        public long TimeFunctionId { get; set; }

        public DateTime TimeFunctionStartDate { get; set; }
        public DateTime TimeFunctionEndDate { get; set; }
        public string? Description { get; set; }


    }
}