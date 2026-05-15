using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PaymentInstallment
{
    public class GetPaymentInstallmentListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد پرداخت")]
        public long PaymentHistoryId { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = " وضعیت پرداخت")]
        public int PayState { get; set; } = 2;

        [Display(Name = " وضعیت اجازه پرداخت")]
        public int AlowState { get; set; } = 2;


    }
}
