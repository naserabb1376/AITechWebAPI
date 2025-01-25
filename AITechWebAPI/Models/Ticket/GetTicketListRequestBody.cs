using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Ticket
{
    public class GetTicketListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

    }
}
