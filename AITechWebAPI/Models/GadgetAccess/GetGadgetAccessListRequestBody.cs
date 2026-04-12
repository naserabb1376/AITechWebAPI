using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GadgetAccess
{
    public class GetGadgetAccessListRequestBody : GetListRequestBody
    {
        [Display(Name = "نام کاربری دسترسی")]
        public string? AccessUserName { get; set; }

        [Display(Name = "شناسه گجت")]
        public string? GadgetKey { get; set; }
    }
}
