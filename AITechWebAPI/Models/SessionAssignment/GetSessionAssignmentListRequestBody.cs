using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.SessionAssignment
{
    public class GetSessionAssignmentListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد جلسه")]
        public long SessionId { get; set; } = 0;
    }
}
