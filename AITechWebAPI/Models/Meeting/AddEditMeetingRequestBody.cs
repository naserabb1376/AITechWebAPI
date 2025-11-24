using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Meeting
{
    public class AddEditMeetingRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "تاریخ جلسه")]
        public string? MeetingTitle { get; set; }

        [Display(Name = "تاریخ جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MeetingDate { get; set; }

        [Display(Name = "ساعت شروع جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MeetingStartTime { get; set; }

        [Display(Name = "وضعیت برگزاری جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MeetingStatus { get; set; }

        [Display(Name = "افراد حاضر در جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MeetingAttendees { get; set; }

        [Display(Name = "برگزار کننده جلسه")]
        public string? MeetingOrganizer { get; set; }


        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
