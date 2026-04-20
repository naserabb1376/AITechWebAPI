using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Comment
{
    public class GetCommentListRequestBody : GetListRequestBody
    {
        [Display(Name = "کلید خارجی")]
        public long ForeignKeyId { get; set; } = 0;

        [Display(Name = "نام شی")]
        public string entityType { get; set; }

        [Display(Name = "کد نظر والد")]
        public long ParentId { get; set; } = 0;

        [Display(Name = "کاربر ایجاد کننده")]
        public long UserId { get; set; } = 0;
    }
}