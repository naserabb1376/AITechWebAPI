using AITechDATA.DataLayer.Repositories;

using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using System.Net.Sockets;
using AITechDATA.CustomResponses;

namespace AITechDATA.DataLayer.Services
{
    public class NewsRep : INewsRep
    {
        private AiITechContext _context;

        public NewsRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddNewsAsync(News news)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.News.AddAsync(news);
                await _context.SaveChangesAsync();
                result.ID = news.ID;
                _context.Entry(news).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditNewsAsync(News news)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.News.Update(news);
                await _context.SaveChangesAsync();
                result.ID = news.ID;
                _context.Entry(news).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistNewsAsync(long newsId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.News
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == newsId);
                result.ID = newsId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<NewsListCustomResponse<News>> GetAllNewsAsync(long userId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            NewsListCustomResponse<News> results = new NewsListCustomResponse<News>();
            try
            {
                var query = _context.News
                    .AsNoTracking()
                    .Where(x =>
                        (userId > 0 && x.UserId == userId) || 
                        ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Content) && x.Content.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Source) && x.Source.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Keywords) && x.Keywords.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.PublishDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();

                results.ResultImages = results.Results
    .ToDictionary(
        user => user,
        user => _context.Images
            .Where(img => img.ForeignKeyId == user.ID && img.EntityType == "News")
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

        public async Task<NewsRowCustomResponse<News>> GetNewsByIdAsync(long newsId)
        {
            NewsRowCustomResponse<News> result = new NewsRowCustomResponse<News>();
            try
            {
                result.Result = await _context.News
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == newsId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<News, List<Image>?>
{
    { result.Result, await _context.Images
        .Where(img => img.ForeignKeyId == newsId && img.EntityType == "News")
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

        public async Task<BitResultObject> RemoveNewsAsync(News news)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Images = _context.Images.Where(img => img.ForeignKeyId == news.ID && img.EntityType == "News");
                _context.Images.RemoveRange(Images);

                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                result.ID = news.ID;
                _context.Entry(news).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveNewsAsync(long newsId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var news = await GetNewsByIdAsync(newsId);
                result = await RemoveNewsAsync(news.Result);
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