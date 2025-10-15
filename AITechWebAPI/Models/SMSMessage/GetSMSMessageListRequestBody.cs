using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.SMSMessage
{
    public class GetSMSMessageListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; } = 0;
    }
}
