using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.PreRegistration
{
    public class GetPreRegistrationListRequestBody : GetListRequestBody
    {
        [Display(Name = "کلید خارجی")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "نام شی")]
        public string EntityType { get; set; } = "";
    }
}
