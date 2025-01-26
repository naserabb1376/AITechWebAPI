using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserCourse
{
    public class GetUserCourseListRequestBody : GetListRequestBody
    {
        [Display(Name = "کد کاربر")]
        public long UserId { get; set; } = 0;

        [Display(Name = "کد درس")]
        public long CourseId { get; set; } = 0;
    }
}
