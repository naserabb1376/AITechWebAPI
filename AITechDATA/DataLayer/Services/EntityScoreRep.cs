using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Services
{
    public class EntityScoreRep : IEntityScoreRep
    {
        private AITechContext _context;

        public EntityScoreRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddEntityScoreAsync(EntityScore EntityScore)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                if (EntityScore.RecordLevel < 2)
                {
                    var existRow = await _context.EntityScores.AnyAsync(x => x.EntityType.ToLower() == EntityScore.EntityType.ToLower() && x.ScoreItemKey.ToLower() == EntityScore.ScoreItemKey.ToLower() && x.RecordLevel < 2);
                    if (existRow)
                    {
                        throw new Exception($"این شاخص ثبت شده است");
                    }
                }
                else
                {
                    var existRow = await _context.EntityScores.AnyAsync(x => x.EntityType.ToLower() == EntityScore.EntityType.ToLower() && x.ScoreItemKey.ToLower() == EntityScore.ScoreItemKey.ToLower() && x.ForeignKeyId == EntityScore.ForeignKeyId && x.RecordLevel == 2);
                    if (existRow)
                    {
                        throw new Exception($"این شاخص برای این داده ثبت شده است");
                    }
                }

                await _context.EntityScores.AddAsync(EntityScore);
                await _context.SaveChangesAsync();
                result.ID = EntityScore.ID;
                _context.Entry(EntityScore).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditEntityScoreAsync(EntityScore EntityScore)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                if (EntityScore.RecordLevel < 2)
                {
                    var existRow = await _context.EntityScores.AnyAsync(x => x.EntityType.ToLower() == EntityScore.EntityType.ToLower() && x.ScoreItemKey.ToLower() == EntityScore.ScoreItemKey.ToLower() && x.RecordLevel < 2 && x.ID != EntityScore.ID);
                    if (existRow)
                    {
                        throw new Exception($"این شاخص ثبت شده است");
                    }
                }
                else
                {
                    var existRow = await _context.EntityScores.AnyAsync(x => x.EntityType.ToLower() == EntityScore.EntityType.ToLower() && x.ScoreItemKey.ToLower() == EntityScore.ScoreItemKey.ToLower() && x.ForeignKeyId == EntityScore.ForeignKeyId && x.RecordLevel == 2 && x.ID != EntityScore.ID);
                    if (existRow)
                    {
                        throw new Exception($"این شاخص برای این داده ثبت شده است");
                    }
                }
                _context.EntityScores.Update(EntityScore);
                await _context.SaveChangesAsync();
                result.ID = EntityScore.ID;
                _context.Entry(EntityScore).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistEntityScoreAsync(long EntityScoreId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.EntityScores
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == EntityScoreId);
                result.ID = EntityScoreId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<EntityScore>> GetAllEntityScoresAsync(long foreignkeyId = 0, string entityType = "", string scoreItemKey = "", long parentId = 0, long userId = 0, int recordLevel = -1, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<EntityScore> results = new ListResultObject<EntityScore>();
            try
            {
                var query = _context.EntityScores.Include(x=> x.User).AsNoTracking();

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(x => x.EntityType.ToLower() == entityType.ToLower());

                if (foreignkeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == foreignkeyId);

                if (!string.IsNullOrEmpty(scoreItemKey))
                    query = query.Where(x => x.ScoreItemKey.ToLower() == scoreItemKey.ToLower());

                if (parentId > 0)
                    query = query.Where(x => x.ScoreItemParentId == parentId);

                if (userId > 0)
                    query = query.Where(x => x.UserId == userId);

                if (recordLevel > -1)
                    query = query.Where(x => x.RecordLevel == recordLevel);

                query = query.Where(x =>
                        ((!string.IsNullOrEmpty(x.EntityType) && x.EntityType.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.ScoreItemKey) && x.ScoreItemKey.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.ScoreItemTitle) && x.ScoreItemTitle.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.TargetObjName) && x.TargetObjName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                        
                        )
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
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

        public async Task<RowResultObject<EntityScore>> GetEntityScoreByIdAsync(long EntityScoreId)
        {
            RowResultObject<EntityScore> result = new RowResultObject<EntityScore>();
            try
            {
                result.Result = await _context.EntityScores
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == EntityScoreId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        public async Task<BitResultObject> RemoveEntityScoreAsync(EntityScore EntityScore)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.EntityScores.Remove(EntityScore);
                await _context.SaveChangesAsync();
                result.ID = EntityScore.ID;
                _context.Entry(EntityScore).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveEntityScoreAsync(long EntityScoreId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var EntityScore = await GetEntityScoreByIdAsync(EntityScoreId);
                result = await RemoveEntityScoreAsync(EntityScore.Result);
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