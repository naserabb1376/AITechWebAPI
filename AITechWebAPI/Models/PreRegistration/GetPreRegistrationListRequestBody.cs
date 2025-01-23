using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PreRegistration
{
    public class GetPreRegistrationListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد گروه")]
        public long GroupId { get; set; } = 0;
    }
}
