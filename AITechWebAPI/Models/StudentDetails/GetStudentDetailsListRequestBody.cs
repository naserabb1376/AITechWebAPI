using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.StudentDetails
{
    public class GetStudentDetailsListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;
    }
}
