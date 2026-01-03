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

    public class GetRegistrationTypesListRequestBody
    {
        public int PageIndex { get; set; } = 1;

        [Display(Name = "اندازه صفحه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int PageSize { get; set; }
        public string SearchText { get; set; } = "";
    }
}
