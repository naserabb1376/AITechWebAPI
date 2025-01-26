using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.UserCourse
{
    public class AddEditUserCourseRequestBody
    {
        public long ID { get; set; } = 0;

        [Display(Name = "کد کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }

        [Display(Name = "کد درس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long CourseId { get; set; }

        [Display(Name = "نوع حضور")]
        public int PeresentType { get; set; }
    }
}
