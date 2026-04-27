using AITechWebAPI.Models.Public;
using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.User
{
    public class GetUserListRequestBody:GetListRequestBody
    {
        public long AddressId { get; set; } = 0;
        //public long RoleId { get; set; } = 0;
        public List<long>? RoleIds { get; set; } = null;
        public long GroupId { get; set; } = 0;
        public long CourseId { get; set; } = 0;
        public long SessionAssignmentId { get; set; } = 0;
        public long SessionId { get; set; } = 0;
        public string StudyField { get; set; } = ""; // رشته تحصیلی
        public string EducationalGrade { get; set; } = ""; // مقطع تحصیلی

    }
}
