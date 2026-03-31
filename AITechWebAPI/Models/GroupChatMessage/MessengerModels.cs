using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GroupChatMessage
{
    public class CanAccessGroupChatRequestBody
    {
        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        //[Display(Name = "کد کاربر ارسال کننده")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        //public long SenderUserId { get; set; }

    }

    public class SendMessageRequestBody
    {
        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        //[Display(Name = "کد کاربر ارسال کننده")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        //public long SenderUserId { get; set; }

        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MessageText { get; set; }

        [Display(Name = "کد پیام مرجع")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? ReplyToMessageId { get; set; }

    }

    public class EditMessageRequestBody
    {

        [Display(Name = "کد پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long MessageId { get; set; }

        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        //[Display(Name = "کد کاربر ارسال کننده")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        //public long SenderUserId { get; set; }

        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MessageText { get; set; }

        //[Display(Name = "کد پیام مرجع")]
        ////[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        //public long? ReplyToMessageId { get; set; }

    }

    public class SoftDeleteMessageRequestBody
    {

        [Display(Name = "کد پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long MessageId { get; set; }

        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        //[Display(Name = "کد کاربر ارسال کننده")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        //public long SenderUserId { get; set; }
    }

    public class GetGroupMessagesRequestBody
    {
        [Display(Name = "کد گروه درسی")]
        public long GroupId { get; set; } = 0;

        public int PageIndex { get; set; } = 1;

        [Display(Name = "اندازه صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int PageSize { get; set; }


    }

    public class UploadChatAttachmentRequestBody
    {
        public IFormFile File { get; set; } = default!;

        [Display(Name = "کد گروه درسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        [Display(Name = "نوع فایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FileType { get; set; }  // "images" یا "files"
        public bool IsPublic { get; set; } = true;      // برای MVP پیشنهاد true

        // اختیاری: اگر خواستی پیام همراه فایل ساخته شود
        [Display(Name = "متن پیام")]
       // [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string? MessageText { get; set; }


    }

    public class SendWithAttachmentResponse
    {
        public long GroupId { get; set; }
        public long MessageId { get; set; }

        public string Url { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public long Size { get; set; }
        public string FileType { get; set; } = null!; // images/files
    }

}
