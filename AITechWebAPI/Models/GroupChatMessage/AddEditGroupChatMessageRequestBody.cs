using AITechDATA.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GroupChatMessage
{
    public class AddEditGroupChatMessageRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MessageText { get; set; }

        [Display(Name = "زمان ارسال پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime SentAt { get; set; }  = DateTime.UtcNow.ToShamsi();

        [Display(Name = "زمان ویرایش پیام")]
        public DateTime? EditedAt { get; set; }

        [Display(Name = "زمان حذف پیام")]
        public DateTime? DeletedAt { get; set; }


        [Display(Name = "کد پیام مرجع")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? ReplyToMessageId { get; set; }

        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        [Display(Name = "کد کاربر ارسال کننده")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long SenderUserId { get; set; }

        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
