using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IContentRep
    {
        Task<ListResultObject<Content>> GetAllContentsAsync(string entityType = "", long ForeignKeyId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<Content>> GetContentByIdAsync(long ContentId);

        Task<BitResultObject> AddContentAsync(Content Content);
        Task<BitResultObject> EditContentAsync(Content Content);

        Task<BitResultObject> RemoveContentAsync(Content Content);
        Task<BitResultObject> RemoveContentAsync(long ContentId);
        Task<BitResultObject> ExistContentAsync(long ContentId);
    }
}