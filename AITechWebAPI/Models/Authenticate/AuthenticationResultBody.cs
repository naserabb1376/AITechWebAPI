using AITechWebAPI.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class AuthenticationResultBody
    {
        public long HserId { get; set; }
        public string FullName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
