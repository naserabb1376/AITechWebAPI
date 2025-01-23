using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Permission
{
    public class GetPermissionListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد نقش")]
        public long RoleId { get; set; } = 0;
    }
}
