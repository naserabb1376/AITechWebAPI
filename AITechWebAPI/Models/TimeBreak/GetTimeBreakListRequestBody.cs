using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.TimeBreak
{
    public class GetTimeBreakListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کارکرد")]
        public long TimeFunctionID { get; set; } = 0;

    }
}