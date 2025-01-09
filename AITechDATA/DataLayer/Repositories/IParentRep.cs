using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IParentRep
    {
        Task<ListResultObject<Parent>> GetAllParentsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<Parent>> GetParentByIdAsync(long parentId);

        Task<BitResultObject> AddParentAsync(Parent parent);

        Task<BitResultObject> EditParentAsync(Parent parent);

        Task<BitResultObject> RemoveParentAsync(Parent parent);

        Task<BitResultObject> RemoveParentAsync(long parentId);

        Task<BitResultObject> ExistParentAsync(long parentId);
    }
}