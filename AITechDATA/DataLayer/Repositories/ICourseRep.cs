using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ICourseRep
    {
        Task<CourseListCustomResponse<Course>> GetAllCoursesAsync(long categoryId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<CourseRowCustomResponse<Course>> GetCourseByIdAsync(long courseId);

        Task<BitResultObject> AddCourseAsync(Course course);

        Task<BitResultObject> EditCourseAsync(Course course);

        Task<BitResultObject> RemoveCourseAsync(Course course);

        Task<BitResultObject> RemoveCourseAsync(long courseId);

        Task<BitResultObject> ExistCourseAsync(long courseId);
    }
}