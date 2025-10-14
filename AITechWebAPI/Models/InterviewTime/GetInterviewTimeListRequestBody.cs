using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.InterviewTime
{
    public class GetInterviewTimeListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد درخواست")]
        public long JobRequestId { get; set; } = 0;

    }
}
