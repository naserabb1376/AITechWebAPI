using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Duty
{
    public class GetDutyChartRequestBody
    {
        [Display(Name = "نوع نمودار")]
        public int ChartType { get; set; } = 1;

        [Display(Name = "از تاریخ")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "تا تاریخ")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "کد شخص")]
        public long UserId { get; set; } = 0;
    }
}
