using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentHistory
{
    public class GetPaymentHistoryListRequestBody : GetListRequestBody
    {
        [Display(Name = "کلید خارجی")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "نام شی")]
        public string EntityType { get; set; } = "";
        public long UserId { get; set; } = 0;
    }
}
