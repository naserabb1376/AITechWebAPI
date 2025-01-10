using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ITeacherResumeRep
    {
        Task<ListResultObject<TeacherResume>> GetAllTeacherResumesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<TeacherResume>> GetTeacherResumeByIdAsync(long teacherResumeId);

        Task<BitResultObject> AddTeacherResumeAsync(TeacherResume teacherResume);

        Task<BitResultObject> EditTeacherResumeAsync(TeacherResume teacherResume);

        Task<BitResultObject> RemoveTeacherResumeAsync(TeacherResume teacherResume);

        Task<BitResultObject> RemoveTeacherResumeAsync(long teacherResumeId);

        Task<BitResultObject> ExistTeacherResumeAsync(long teacherResumeId);
    }
}