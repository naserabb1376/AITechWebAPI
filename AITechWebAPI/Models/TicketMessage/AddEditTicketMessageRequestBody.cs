using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TicketMessage
{
    public class AddEditTicketMessageRequestBody
    {
        public long ID { get; set; } = 0;


        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string MessageContent { get; set; }

        [Display(Name = "کد تیکت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long TicketId { get; set; }

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1, long.MaxValue, ErrorMessage = "مقدار {0} باید بزرگتر از 0 باشد")]
        public long UserId { get; set; }

        public bool IsAdminResponse { get; set; }
    }
}
