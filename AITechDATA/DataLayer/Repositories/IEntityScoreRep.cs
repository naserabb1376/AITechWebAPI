using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IEntityScoreRep
    {
        Task<ListResultObject<EntityScore>> GetAllEntityScoresAsync(long foreignkeyId = 0,string entityType ="", string scoreItemKey = "",long parentId = 0,long userId = 0,int recordLevel = -1, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<EntityScore>> GetEntityScoreByIdAsync(long EntityScoreId);

        Task<BitResultObject> AddEntityScoreAsync(EntityScore EntityScore);

        Task<BitResultObject> EditEntityScoreAsync(EntityScore EntityScore);

        Task<BitResultObject> RemoveEntityScoreAsync(EntityScore EntityScore);

        Task<BitResultObject> RemoveEntityScoreAsync(long EntityScoreId);

        Task<BitResultObject> ExistEntityScoreAsync(long EntityScoreId);
    }
}