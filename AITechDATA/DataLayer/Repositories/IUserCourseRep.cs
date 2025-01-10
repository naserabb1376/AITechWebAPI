using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IUserCourseRep
    {
        Task<ListResultObject<UserCourse>> GetAllUserCoursesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<UserCourse>> GetUserCourseByIdAsync(long UserCourseId);

        Task<BitResultObject> AddUserCourseAsync(UserCourse UserCourse);

        Task<BitResultObject> EditUserCourseAsync(UserCourse UserCourse);

        Task<BitResultObject> RemoveUserCourseAsync(UserCourse UserCourse);

        Task<BitResultObject> RemoveUserCourseAsync(long UserCourseId);

        Task<BitResultObject> ExistUserCourseAsync(long UserCourseId);
    }
}