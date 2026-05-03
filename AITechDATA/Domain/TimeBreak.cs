using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    public class TimeBreak : BaseEntity
    {
        public TimeSpan TimeBreakStartTime { get; set; }
        public TimeSpan TimeBreakEndTime { get; set; }
        public long TimeFunctionId { get; set; } 
        public TimeFunction TimeFunction { get; set; }
        public string? Description { get; set; }
    }
}