using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.Group
{
    public class GetGroupListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد درس")]
        public long CourseId { get; set; } = 0;

        [Display(Name = "کد دانشجو")]
        public long StudentId { get; set; } = 0;

        [Display(Name = "کد مدرس")]
        public long TeacherId { get; set; } = 0;

        [Display(Name = "وضعیت گروه")]
        public string GroupStatus { get; set; } = "";
    }
}