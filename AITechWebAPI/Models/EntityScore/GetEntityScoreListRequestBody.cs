using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.EntityScore
{
    public class GetEntityScoreListRequestBody : GetListRequestBody
    {
        [Display(Name = "کلید خارجی")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "نام شی")]
        public string EntityType { get; set; } = "";

        [Display(Name = "کلید شاخص")]
        public string ScoreItemKey { get; set; } = "";

        [Display(Name = "کد والد")]
        public long ParentId { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "سطح رکورد")]
        public int RecordLevel { get; set; } = -1;

    }
}
