using AITechDATA.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GroupChatReadState
{
    public class AddEditGroupChatReadStateRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "زمان آخرین بازدید")]
        public DateTime? LastReadAt { get; set; }

        [Display(Name = "کد آخرین پیام خوانده شده")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? LastReadMessageId { get; set; }

        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        // ✅ Attachment
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public string? AttachmentType { get; set; } // "images" | "files"

        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
