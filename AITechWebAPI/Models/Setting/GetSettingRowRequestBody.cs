using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Setting
{
    public class GetSettingRowRequestBody : GetListRequestBody
    {
        [Display(Name = "کد")]
        public long ID { get; set; } = 0;

        [Display(Name = "کلید")]
        public string Key { get; set; } = "";
    }
}
