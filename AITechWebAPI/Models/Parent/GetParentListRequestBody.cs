using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Parent
{
    public class GetParentListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد دانش آموز")]
        public long StudentDetailsId { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;
    }
}
