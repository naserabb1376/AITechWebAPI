using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentHistory
{
    public class GetPaymentHistoryListRequestBody : GetListRequestBody
    {
        public long GroupId { get; set; } = 0;
        public long UserId { get; set; } = 0;
    }
}
