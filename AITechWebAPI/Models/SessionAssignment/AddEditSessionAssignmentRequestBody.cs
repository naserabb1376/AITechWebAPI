using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.SessionAssignment
{
    public class AddEditSessionAssignmentRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "عنوان تمرین جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; } 

        [Display(Name = "شرح تمرین جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "مهلت ارسال تمرین")]
        public string? DueDate { get; set; }

        [Display(Name = "کد جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long SessionId { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
