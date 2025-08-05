
using AITechWebAPI.ViewModels;

namespace AITechWebAPI.Models.Authenticate
{
    public class AuthenticationResultBody
    {
        public UserVM User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
