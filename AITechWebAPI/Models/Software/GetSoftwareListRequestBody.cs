using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Software
{
    public class GetSoftwareListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد دسته بندی")]
        public long CategoryId { get; set; } = 0;
    }
}

