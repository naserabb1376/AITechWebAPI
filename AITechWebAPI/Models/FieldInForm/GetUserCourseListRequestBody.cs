using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.FieldInForm
{
    public class GetFieldInFormListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد فرم")]
        public long SubmitFormId { get; set; } = 0;

        [Display(Name = "کد فیلد")]
        public long FormFieldId { get; set; } = 0;
    }
}
