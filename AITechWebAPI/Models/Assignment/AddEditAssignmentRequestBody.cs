using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Assignment
{
    public class AddEditAssignmentRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان تمرین")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } 

        [Display(Name = "شرح تمرین")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; } 

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        [Display(Name = "کد تمرین در جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long SessionAssignmentId { get; set; }
        
        [Display(Name = "تاریخ ارسال تمرین")]
        public string? SubmissionDate { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
