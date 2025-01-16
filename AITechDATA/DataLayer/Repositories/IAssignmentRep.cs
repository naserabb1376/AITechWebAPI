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
        Task<ListResultObject<Assignment>> GetAllAssignmentsAsync(long userId=0,long sessionAssignmentId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Assignment>> GetAssignmentByIdAsync(long assignmentId);

        Task<BitResultObject> AddAssignmentsAsync(List<Assignment> assignments);

        Task<BitResultObject> EditAssignmentsAsync(List<Assignment> assignments);

        Task<BitResultObject> RemoveAssignmentsAsync(List<Assignment> assignments);

        Task<BitResultObject> RemoveAssignmentsAsync(List<long> assignmentIds);

        Task<BitResultObject> ExistAssignmentAsync(long assignmentId);
    }
}