using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PermissionRole
{
    public class GetPermissionRoleListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد نقش")]
        public long RoleId { get; set; } = 0;

        [Display(Name = "کد دسترسی")]
        public long PermissionId { get; set; } = 0;
    }
}
