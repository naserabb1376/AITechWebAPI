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
    public class LinkedEntityRep : ILinkedEntityRep
    {
        private AITechContext _context;

        public LinkedEntityRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddLinkedEntityAsync(LinkedEntity LinkedEntity)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.LinkedEntities.AddAsync(LinkedEntity);
                await _context.SaveChangesAsync();
                result.ID = LinkedEntity.ID;
                _context.Entry(LinkedEntity).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditLinkedEntityAsync(LinkedEntity LinkedEntity)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.LinkedEntities.Update(LinkedEntity);
                await _context.SaveChangesAsync();
                result.ID = LinkedEntity.ID;
                _context.Entry(LinkedEntity).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistLinkedEntityAsync(long LinkedEntityId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.LinkedEntities
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == LinkedEntityId);
                result.ID = LinkedEntityId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<LinkedEntity>> GetAllLinkedEntitiesAsync(
      string entityName = "", long ForeignKeyId = 0,long LinkedEntityId=0, long creatorId = 0,
      int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<LinkedEntity> results = new ListResultObject<LinkedEntity>();

            try
            {
                var query = _context.LinkedEntities.AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن
                if (ForeignKeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == ForeignKeyId);

                if (LinkedEntityId > 0)
                    query = query.Where(x => x.LinkedEntityId == LinkedEntityId);

                if (creatorId > 0)
                    query = query.Where(x => x.CreatorId == creatorId);


                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(x => x.EntityName == entityName);

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.LinkType) && x.LinkType.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderBy(x => x.Priority)
                    .SortBy(sortQuery)
                    .ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }


      

        public async Task<RowResultObject<LinkedEntity>> GetLinkedEntityByIdAsync(long LinkedEntityId)
        {
            RowResultObject<LinkedEntity> result = new RowResultObject<LinkedEntity>();
            try
            {
                result.Result = await _context.LinkedEntities
                    .AsNoTracking()
                    //.Include(x => x.Assignment)
                    .SingleOrDefaultAsync(x => x.ID == LinkedEntityId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        public async Task<BitResultObject> RemoveLinkedEntityAsync(LinkedEntity LinkedEntity)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.LinkedEntities.Remove(LinkedEntity);
                await _context.SaveChangesAsync();
                result.ID = LinkedEntity.ID;
                _context.Entry(LinkedEntity).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveLinkedEntityAsync(long LinkedEntityId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var LinkedEntity = await GetLinkedEntityByIdAsync(LinkedEntityId);
                result = await RemoveLinkedEntityAsync(LinkedEntity.Result);
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