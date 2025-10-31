using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IAwardRep
    {
        Task<ListResultObject<Award>> GetAllAwardsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<Award>> GetAwardByIdAsync(long AwardId);

        Task<BitResultObject> AddAwardAsync(Award Award);

        Task<BitResultObject> EditAwardAsync(Award Award);

        Task<BitResultObject> RemoveAwardAsync(Award Award);

        Task<BitResultObject> RemoveAwardAsync(long AwardId);

        Task<BitResultObject> ExistAwardAsync(long AwardId);
    }
}