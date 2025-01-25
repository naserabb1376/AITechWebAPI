using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TicketMessage
{
    public class GetTicketMessageListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد تیکت")]
        public long TicketId { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

    }
}
