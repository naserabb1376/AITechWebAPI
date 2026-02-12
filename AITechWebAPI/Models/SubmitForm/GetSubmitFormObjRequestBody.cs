using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.SubmitForm
{
    public class GetSubmitFormObjRequestBody
    {
        [Display(Name = "فیلد مورد جستجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SearchField { get; set; }

        [Display(Name = "مقدار مورد جستجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string SearchValue { get; set; }

    }
}