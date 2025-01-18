using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Assignment
{
    public class GetAssignmentListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد تمرین در جلسه")]
        public long SessionAssignmentId { get; set; } = 0;
    }
}
