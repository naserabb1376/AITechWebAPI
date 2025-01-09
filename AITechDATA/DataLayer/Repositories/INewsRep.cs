using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface INewsRep
    {
        Task<ListResultObject<News>> GetAllNewsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<News>> GetNewsByIdAsync(long newsId);

        Task<BitResultObject> AddNewsAsync(News news);

        Task<BitResultObject> EditNewsAsync(News news);

        Task<BitResultObject> RemoveNewsAsync(News news);

        Task<BitResultObject> RemoveNewsAsync(long newsId);

        Task<BitResultObject> ExistNewsAsync(long newsId);
    }
}