using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.DiscountTarget
{
    public class GetDiscountTargetListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد تخفیف")]
        public long DiscountId { get; set; } = 0;

        [Display(Name = "کد مشمول تخفیف")]
        public long TargetId { get; set; } = 0;


    }
}
