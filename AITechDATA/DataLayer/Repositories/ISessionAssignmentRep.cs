using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ISessionAssignmentRep
    {
        Task<ListResultObject<SessionAssignment>> GetAllSessionAssignmentsAsync(long SessionId = 0,long userId=0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<SessionAssignment>> GetSessionAssignmentByIdAsync(long sessionAssignmentId);

        Task<BitResultObject> AddSessionAssignmentAsync(SessionAssignment sessionAssignment);

        Task<BitResultObject> EditSessionAssignmentAsync(SessionAssignment sessionAssignment);

        Task<BitResultObject> RemoveSessionAssignmentAsync(SessionAssignment sessionAssignment);

        Task<BitResultObject> RemoveSessionAssignmentAsync(long sessionAssignmentId);

        Task<BitResultObject> ExistSessionAssignmentAsync(long sessionAssignmentId);
    }
}