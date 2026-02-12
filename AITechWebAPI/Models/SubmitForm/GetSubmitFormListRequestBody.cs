using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.SubmitForm
{
    public class GetSubmitFormListRequestBody : GetListRequestBody
    {
        [Display(Name = "نام جدول")]
        public string EntityName { get; set; } = "";

        [Display(Name = "کاربر ایجاد کننده")]
        public long CreatorId { get; set; } = 0;
    }
}