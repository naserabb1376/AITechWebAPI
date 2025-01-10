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

namespace AITechDATA.DataLayer.Servisces
{
    public class NewsRep : INewsRep
    {
        private AiITechContext _context;

        public NewsRep()
        {
            _context = DbTools.GetDbContext();
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

        public async Task<ListResultObject<News>> GetAllNewsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<News> results = new ListResultObject<News>();
            try
            {
                var query = _context.News
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Content) && x.Content.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Source) && x.Source.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Keywords) && x.Keywords.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.PublishDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<News>> GetNewsByIdAsync(long newsId)
        {
            RowResultObject<News> result = new RowResultObject<News>();
            try
            {
                result.Result = await _context.News
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == newsId);
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