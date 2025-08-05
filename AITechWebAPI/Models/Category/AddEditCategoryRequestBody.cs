using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Category
{
    public class AddEditCategoryRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان دسته بندی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CategoryName { get; set; }

        [Display(Name = "شرح دسته بندی")]
        public string? CategoryDescription { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
