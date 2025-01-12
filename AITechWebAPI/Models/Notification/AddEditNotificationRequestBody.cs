using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Notification
{
    public class AddEditNotificationRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد شخص")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserID { get; set; }

        [Display(Name = "متن اعلان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Message { get; set; }

        //[Display(Name = "تاریخ اعلان")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //public DateTime? SentDate { get; set; }
        
        [Display(Name = "وضعیت")]
        public bool NotificationSeenStatus { get; set; }
       // public string? Description { get; set; }
    }
}
