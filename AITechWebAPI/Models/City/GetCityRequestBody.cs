using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechDATA.Models.City
{
    public class GetCityListRequestBody:GetListRequestBody
    {
        [Display(Name = "کد والد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long ParentId { get; set; } = -1;

    }
}
