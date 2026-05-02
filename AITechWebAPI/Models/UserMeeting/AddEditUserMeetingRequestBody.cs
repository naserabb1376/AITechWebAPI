using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserMeeting
{
    public class AddEditUserMeetingRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }

        [Display(Name = "کد جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long MeetingId { get; set; }

    }
}
