using AITechWebAPI.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class RefreshTokenResultBody
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }
}
