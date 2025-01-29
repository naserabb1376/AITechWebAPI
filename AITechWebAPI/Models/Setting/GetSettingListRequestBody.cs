using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Setting
{
    public class GetSettingListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد والد")]
        public long ParentId { get; set; } = 0;

        [Display(Name = "کلید")]
        public string Key { get; set; } = "";
    }
}
