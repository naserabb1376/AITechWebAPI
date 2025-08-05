using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Ticket
{
    public class AddEditTicketRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "موضوع تیکت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Subject { get; set; } 

        [Display(Name = "شرح تیکت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        [Display(Name = "زبان های دیگر")]
        public string? OtherLangs { get; set; } = "";

    }
}
