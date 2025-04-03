using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Image
{
    public class GetImageListRequestBody : GetListRequestBody
    {
        [Display(Name = "کلید خارجی")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "نام شی")]
        public string EntityType { get; set; } = "";

        [Display(Name = "کاربر ایجاد کننده")]
        public long CreatorId { get; set; } = 0;
    }
}
