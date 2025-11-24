using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Minutes
{
    public class GetMinutesListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد جلسه")]
        public long MeetingId { get; set; } = 0;
    }
}