using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Dismissal
{
    public class GetDismissalListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد شخص")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد بررسی کننده")]
        public long CheckerUserId { get; set; } = 0;

        [Display(Name = "وضعیت تایید")]
        public int ApproveState { get; set; } = 2;
    }
}
