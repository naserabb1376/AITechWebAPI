using User= AITechDATA.Domain.User;
using AITechWebAPI.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class AuthenticationResultBody
    {
        public AITechDATA.Domain.User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
