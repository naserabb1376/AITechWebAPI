using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GroupChatReadState
{
    public class GetReadStateRequestBody
    {
        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }
    }

    public class SeenRequest
    {
        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        [Display(Name = "کد آخرین پیام خوانده شده")]
        public long LastReadMessageId { get; set; }
    }
}
