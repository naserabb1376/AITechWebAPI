using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GroupChatReadState
{
    public class GetGroupChatReadStateListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد گروه درسی")]
        public long GroupId { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        public long? UserId { get; set; }

        [Display(Name = "کد آخرین پیام خوانده شده")]
        public long LastReadMessageId { get; set; } = 0;

    }
}
