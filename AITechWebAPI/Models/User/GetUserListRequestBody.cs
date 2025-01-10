using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.User
{
    public class GetUserListRequestBody:GetListRequestBody
    {
        public long AddressId { get; set; } = 0;
        public long RoleId { get; set; } = 0;
    }
}
