using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Article
{
    public class AddEditArticleRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان مقاله")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } 

        [Display(Name = "شرح مقاله")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "نام نویسنده")]
        public string? AuthorName { get; set; }

        [Display(Name = "کد دسته بندی")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long? CategoryId { get; set; }

        [Display(Name = "توضیحات")]
        public string? Note { get; set; }

        [Display(Name = "وضعیت فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
