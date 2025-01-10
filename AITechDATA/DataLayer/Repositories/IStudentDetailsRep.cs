using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IStudentDetailsRep
    {
        Task<ListResultObject<StudentDetails>> GetAllStudentDetailsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<StudentDetails>> GetStudentDetailsByIdAsync(long studentDetailsId);

        Task<BitResultObject> AddStudentDetailsAsync(StudentDetails studentDetails);

        Task<BitResultObject> EditStudentDetailsAsync(StudentDetails studentDetails);

        Task<BitResultObject> RemoveStudentDetailsAsync(StudentDetails studentDetails);

        Task<BitResultObject> RemoveStudentDetailsAsync(long studentDetailsId);

        Task<BitResultObject> ExistStudentDetailsAsync(long studentDetailsId);
    }
}