using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Domain
{
    // JobRequest: جدول درخواست شغل
    public class InterviewTime : BaseEntity
    {
        public string InterviewDate { get; set; }
        public string InterviewStartTime { get; set; }
        public string InterviewEndTime { get; set; }
        public long JobRequestId { get; set; }
        public JobRequest JobRequest { get; set; }
    }

    public class InterviewSlot
    {
        public string InterviewDate { get; set; }
        public string InterviewStartTime { get; set; }
        public string InterviewEndTime { get; set; }
        public bool IsReserved { get; set; }
    }
}