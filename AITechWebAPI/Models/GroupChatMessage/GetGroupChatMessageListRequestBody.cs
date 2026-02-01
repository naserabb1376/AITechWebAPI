using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GroupChatMessage
{
    public class GetGroupChatMessageListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد گروه درسی")]
        public long GroupId { get; set; } = 0;

        [Display(Name = "کد کاربر ارسال کننده")]
        public long SenderUserId { get; set; } = 0;

        [Display(Name = "کد پیام مرجع")]
        public long ReplyToMessageId { get; set; } = 0;

        [Display(Name = "نمایش پیامهای حذف شده")]
        public bool WithDeleted { get; set; } = false;


    }
}
