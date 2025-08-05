using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ILinkedEntityRep
    {
        Task<ListResultObject<LinkedEntity>> GetAllLinkedEntitiesAsync(string entityName = "", long ForeignKeyId = 0, long LinkedEntityId = 0, long creatorId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<LinkedEntity>> GetLinkedEntityByIdAsync(long LinkedEntityId);

        Task<BitResultObject> AddLinkedEntityAsync(LinkedEntity LinkedEntity);
        Task<BitResultObject> EditLinkedEntityAsync(LinkedEntity LinkedEntity);

        Task<BitResultObject> RemoveLinkedEntityAsync(LinkedEntity LinkedEntity);
        Task<BitResultObject> RemoveLinkedEntityAsync(long LinkedEntityId);
        Task<BitResultObject> ExistLinkedEntityAsync(long LinkedEntityId);
    }
}