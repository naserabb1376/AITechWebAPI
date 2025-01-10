using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ISessionRep
    {
        Task<ListResultObject<Session>> GetAllSessionsAsync(long GroupId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<Session>> GetSessionByIdAsync(long sessionId);

        Task<BitResultObject> AddSessionAsync(Session session);

        Task<BitResultObject> EditSessionAsync(Session session);

        Task<BitResultObject> RemoveSessionAsync(Session session);

        Task<BitResultObject> RemoveSessionAsync(long sessionId);

        Task<BitResultObject> ExistSessionAsync(long sessionId);
    }
}