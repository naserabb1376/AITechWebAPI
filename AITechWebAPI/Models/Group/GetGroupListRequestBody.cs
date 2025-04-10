using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Group
{
    public class GetGroupListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد درس")]
        public long CourseId { get; set; } = 0;

        [Display(Name = "آیدی کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "وضعیت گروه")]
        public string GroupStatus { get; set; } = "";
    }
}