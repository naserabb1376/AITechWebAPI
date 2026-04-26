using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IDismissalRep
    {
        Task<ListResultObject<Dismissal>> GetAllDismissalsAsync(long userId = 0, long checkerUserId = 0,int approveState=2, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Dismissal>> GetDismissalByIdAsync(long DismissalId);

        Task<BitResultObject> AddDismissalAsync(Dismissal Dismissal);

        Task<BitResultObject> EditDismissalAsync(Dismissal Dismissal);

        Task<BitResultObject> RemoveDismissalAsync(Dismissal Dismissal);

        Task<BitResultObject> RemoveDismissalAsync(long DismissalId);

        Task<BitResultObject> ExistDismissalAsync(long DismissalId);
    }
}
