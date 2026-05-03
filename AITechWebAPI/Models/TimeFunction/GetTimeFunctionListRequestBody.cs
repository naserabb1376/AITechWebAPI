using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TimeFunction
{
    public class GetTimeFunctionListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد شخص")]
        public long UserId { get; set; } = 0;

        [Display(Name = "تاریخ شروع کارکرد")]
        public string? TimeFunctionStartDate { get; set; } = "";

        [Display(Name = "تاریخ پایان کارکرد")]
        public string? TimeFunctionEndDate { get; set; } = "";

    }
}