using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Duty
{
    public class GetDutyListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد شخص")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد ارسال کننده")]
        public long SenderUserId { get; set; } = 0;

        [Display(Name = " وضعیت خوانده شدن")]
        public int ReadState { get; set; } = 2;

        [Display(Name = " وضعیت انجام شدن")]
        public int DoneState { get; set; } = 2;
    }
}
