using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LinkedEntity
{
    public class GetLinkedEntityListRequestBody : GetListRequestBody
    {
        [Display(Name = "کلید خارجی")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "کد رکورد مرتبط")]
        public long LinkedEntityId { get; set; } = 0;

        [Display(Name = "نام شی")]
        public string EntityName { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long CreatorId { get; set; } = 0;
    }
}