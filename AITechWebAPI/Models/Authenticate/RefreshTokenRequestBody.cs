using AITechWebAPI.Tools;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Authenticate
{
    public class RefreshTokenRequestBody
    {
        [Display(Name = "رفرش توکن")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RefreshToken { get; set; }
    }
}
