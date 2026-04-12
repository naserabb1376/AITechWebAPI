using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.GadgetAccess
{
    public class GetAccessableGadgetsRequestBody
    {
        [Display(Name = "نام کاربری دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AccessUserName { get; set; }

        [Display(Name = "رمز عبور دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string AccessPassword { get; set; }

        [Display(Name = "شناسه گجت")]
        public string? GadgetKey { get; set; }

    }
}
