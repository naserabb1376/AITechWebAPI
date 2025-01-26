using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserGroup
{
    public class GetUserGroupListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد گروه")]
        public long GroupId { get; set; } = 0;
    }
}
