using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.CustomResponses;

namespace AITechDATA.DataLayer.Services
{
    public class ArticleRep : IArticleRep
    {
        private AiITechContext _context;

        public ArticleRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddArticleAsync(Article Article)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Articles.AddAsync(Article);
                await _context.SaveChangesAsync();
                result.ID = Article.ID;
                _context.Entry(Article).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditArticleAsync(Article Article)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Articles.Update(Article);
                await _context.SaveChangesAsync();
                result.ID = Article.ID;
                _context.Entry(Article).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistArticleAsync(long ArticleId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Articles
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == ArticleId);
                result.ID = ArticleId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ArticleListCustomResponse<Article>> GetAllArticlesAsync(long categoryId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ArticleListCustomResponse<Article> results = new ArticleListCustomResponse<Article>();
            try
            {
                var query = _context.Articles
                    .AsNoTracking()
                    .Where(x =>
                        (categoryId > 0 && x.CategoryId == categoryId)
                        ||((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Category)
                    .ToListAsync();

                // Map images for each Article
                results.ResultImages = results.Results
                    .ToDictionary(
                        user => user,
                        user => _context.Images
                            .Where(img => img.ForeignKeyId == user.ID && img.EntityType == "Article")
                            .ToList()
                    );
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<ArticleRowCustomResponse<Article>> GetArticleByIdAsync(long ArticleId)
        {
            ArticleRowCustomResponse<Article> result = new ArticleRowCustomResponse<Article>();
            try
            {
                result.Result = await _context.Articles
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .SingleOrDefaultAsync(x => x.ID == ArticleId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<Article, List<Image>?>
            {
                { result.Result, await _context.Images
                    .Where(img => img.ForeignKeyId == ArticleId && img.EntityType == "Article")
                    .ToListAsync() }
            };
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveArticleAsync(Article Article)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Images = _context.Images.Where(img => img.ForeignKeyId == Article.ID && img.EntityType == "Article");
                _context.Images.RemoveRange(Images);

                _context.Articles.Remove(Article);
                await _context.SaveChangesAsync();
                result.ID = Article.ID;
                _context.Entry(Article).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveArticleAsync(long ArticleId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Article = await GetArticleByIdAsync(ArticleId);
                result = await RemoveArticleAsync(Article.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }
    }
}