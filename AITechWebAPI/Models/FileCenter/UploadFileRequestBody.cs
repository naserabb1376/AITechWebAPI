using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.FileCenter
{
    public class UploadFileRequestBody
    {
        [Display(Name = "کد رکورد مرتبط با فایل")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long RowId { get; set; } = 0;

        [Display(Name = "نام شی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string EntityName { get; set; }

        [Display(Name = "نوع فایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FileType { get; set; }

        [Display(Name = "سطح دسترسی فایل")]
        public bool IsPublic { get; set; }

        [Display(Name = "برچسب")]
        public string? Tag { get; set; }

        [Display(Name = "توضیحات")]
        public string? Note { get; set; }

    }
}