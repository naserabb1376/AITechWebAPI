using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Parent
{
    public class AddEditParentRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "نام ولی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; } 

        [Display(Name = "شغل ولی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Job { get; set; }

        [Display(Name = "شماره تماس ولی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]

        public string ContactNumber { get; set; }

        [Display(Name = "کد دانش آموز")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long StudentDetailsId { get; set; }
    }
}
