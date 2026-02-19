using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Notification
{
    public class GetNotificationListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد شخص")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد ارسال کننده")]
        public long SenderUserId { get; set; } = 0;
    }
}
