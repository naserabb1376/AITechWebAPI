using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Session
{
    public class AddEditSessionRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long GroupId { get; set; }

        [Display(Name = "تاریخ جلسه")]
        public string? SessionDate { get; set; }
    }
}
