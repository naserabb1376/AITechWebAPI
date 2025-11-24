using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Minutes
{
    public class AddEditMinutesRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "موضوع صورتجلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MinutesSubject { get; set; }

        [Display(Name = "متن صورتجلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MinutesDescription { get; set; }

        [Display(Name = "کد جلسه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long MeetingId { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";


    }
}
