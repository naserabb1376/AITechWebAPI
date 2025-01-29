using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LoginMethod
{
    public class GetLoginMethodListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

    }
}
