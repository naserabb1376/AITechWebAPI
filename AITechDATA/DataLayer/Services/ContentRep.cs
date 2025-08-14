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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace AITechDATA.DataLayer.Services
{
    public class ContentRep : IContentRep
    {
        private AiITechContext _context;

        public ContentRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddContentAsync(Content Content)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Contents.AddAsync(Content);
                await _context.SaveChangesAsync();
                result.ID = Content.ID;
                _context.Entry(Content).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditContentAsync(Content Content)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Contents.Update(Content);
                await _context.SaveChangesAsync();
                result.ID = Content.ID;
                _context.Entry(Content).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistContentAsync(long ContentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Contents
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == ContentId);
                result.ID = ContentId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Content>> GetAllContentsAsync(
      string entityType = "", long ForeignKeyId = 0,
      int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<Content> results = new ListResultObject<Content>();

            try
            {
                var query = _context.Contents.AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن
                if (ForeignKeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == ForeignKeyId);


                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(x => x.EntityType == entityType);

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderByDescending(x => x.CreateDate)
                    .SortBy(sortQuery)
                    .ToPaging(pageIndex, pageSize)
                    //.Include(x => x.Assignment) // در صورت نیاز بازکنید
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }


      

        public async Task<RowResultObject<Content>> GetContentByIdAsync(long ContentId)
        {
            RowResultObject<Content> result = new RowResultObject<Content>();
            try
            {
                result.Result = await _context.Contents
                    .AsNoTracking()
                    //.Include(x => x.Assignment)
                    .SingleOrDefaultAsync(x => x.ID == ContentId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        public async Task<BitResultObject> RemoveContentAsync(Content Content)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Contents.Remove(Content);
                await _context.SaveChangesAsync();
                result.ID = Content.ID;
                _context.Entry(Content).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveContentAsync(long ContentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Content = await GetContentByIdAsync(ContentId);
                result = await RemoveContentAsync(Content.Result);
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