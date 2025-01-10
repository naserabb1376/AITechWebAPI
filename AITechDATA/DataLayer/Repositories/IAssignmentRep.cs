using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IAssignmentRep
    {
        Task<ListResultObject<Assignment>> GetAllAssignmentsAsync(long sessionAssignmentId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Assignment>> GetAssignmentByIdAsync(long assignmentId);

        Task<BitResultObject> AddAssignmentAsync(Assignment assignment);

        Task<BitResultObject> EditAssignmentAsync(Assignment assignment);

        Task<BitResultObject> RemoveAssignmentAsync(Assignment assignment);

        Task<BitResultObject> RemoveAssignmentAsync(long assignmentId);

        Task<BitResultObject> ExistAssignmentAsync(long assignmentId);
    }
}