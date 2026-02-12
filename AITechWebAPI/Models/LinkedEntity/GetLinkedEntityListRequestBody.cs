using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.LinkedEntity
{
    public class GetLinkedEntityListRequestBody : GetListRequestBody
    {
        [Display(Name = "نوع خروجی")]
        public string TargetType { get; set; } = "link";

        [Display(Name = "کد رکورد مبدا")]
        public long SourceRowId { get; set; } = 0;

        [Display(Name = "کد رکورد مقصد")]
        public long DestRowId { get; set; } = 0;

        [Display(Name = "نام جدول مبدا")]
        public string SourceTableName { get; set; }

        [Display(Name = "نام جدول مقصد")]
        public string DestTableName { get; set; }

        [Display(Name = "نوع ارتباط")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string LinkType { get; set; }

        [Display(Name = "کاربر ایجاد کننده")]
        public long CreatorId { get; set; } = 0;
    }
}