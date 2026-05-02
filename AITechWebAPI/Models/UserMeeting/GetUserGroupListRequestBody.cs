using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserMeeting
{
    public class GetUserMeetingListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد جلسه")]
        public long MeetingId { get; set; } = 0;
    }
}
