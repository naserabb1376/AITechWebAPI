using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IUserCourseRep
    {
        Task<ListResultObject<UserCourse>> GetAllUserCoursesAsync(long UserId = 0, long CourseId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<UserCourse>> GetUserCourseByIdAsync(long UserCourseId);

        Task<BitResultObject> AddUserCoursesAsync(List<UserCourse> UserCourses);

        Task<BitResultObject> EditUserCoursesAsync(List<UserCourse> UserCourses);

        Task<BitResultObject> RemoveUserCoursesAsync(List<UserCourse> UserCourses);

        Task<BitResultObject> RemoveUserCoursesAsync(List<long> UserCourseIds);

        Task<BitResultObject> ExistUserCourseAsync(long UserCourseId);
    }
}