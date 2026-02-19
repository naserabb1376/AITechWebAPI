using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Discount
{
    public class GetDiscountListRequestBody : GetListRequestBody
    {

        [Display(Name = "کد رکورد")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "نام موجودیت تخفیف دار")]
        public string EntityName { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long CreatorId { get; set; } = 0;
    }
}