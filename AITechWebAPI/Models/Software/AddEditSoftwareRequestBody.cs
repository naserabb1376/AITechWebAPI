using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Software
{
    public class AddEditSoftwareRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان نرم افزار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        [Display(Name = "توضیحات نرم افزار")]
        public string? Description { get; set; }

        [Display(Name = "کد دسته بندی")]
        public long? CategoryId { get; set; }

        [Display(Name = "نکته")]
        public string? Note { get; set; }

        [Display(Name = "لینک دانلود")]
        public string? DownloadUrl { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";
    }
}
