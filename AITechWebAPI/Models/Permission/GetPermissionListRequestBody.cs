using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Permission
{
    public class GetPermissionListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد نقش")]
        public long RoleId { get; set; } = 0;

        [Display(Name = "نوع دسترسی")]
        public string? PermissionType { get; set; } = "";
    }
}
