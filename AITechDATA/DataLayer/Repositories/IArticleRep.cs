using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IArticleRep
    {
        Task<ArticleListCustomResponse<Article>> GetAllArticlesAsync(long categoryId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<ArticleRowCustomResponse<Article>> GetArticleByIdAsync(long ArticleId);

        Task<BitResultObject> AddArticleAsync(Article Article);

        Task<BitResultObject> EditArticleAsync(Article Article);

        Task<BitResultObject> RemoveArticleAsync(Article Article);

        Task<BitResultObject> RemoveArticleAsync(long ArticleId);

        Task<BitResultObject> ExistArticleAsync(long ArticleId);
    }
}