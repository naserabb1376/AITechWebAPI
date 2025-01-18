using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.AdminReport
{
    public class GetAdminReportListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربری مدیر")]
        public long AdminId { get; set; } = 0;
    }
}
