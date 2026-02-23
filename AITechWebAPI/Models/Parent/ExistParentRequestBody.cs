using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Parent
{
    public class ExistParentRequestBody
    {
        [Display(Name = "مقدار جستجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FieldValue { get; set; }

        [Display(Name = "ستون جستجو")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FieldName { get; set; }

    }
}
