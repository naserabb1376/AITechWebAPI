using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Course
{
    public class AddEditCourseRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان درس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } 

        [Display(Name = "شرح درس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; } 

        [Display(Name = "کد دسته بندی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long CategoryId { get; set; }
    }
}
