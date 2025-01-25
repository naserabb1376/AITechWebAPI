using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.StudentDetails
{
    public class AddEditStudentDetailsRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

    }
}
