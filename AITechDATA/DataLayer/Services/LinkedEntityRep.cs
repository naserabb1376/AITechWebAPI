using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
      string sourcetableName = "", string desttableName = "", long sourceRowId = 0, long destRowId = 0, string linkType = "", long creatorId = 0,
      int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<LinkedEntity> results = new ListResultObject<LinkedEntity>();

            try
            {
                var query = _context.LinkedEntities.AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن

                if (!string.IsNullOrEmpty(sourcetableName))
                    query = query.Where(x => x.SourceTableName.ToLower() == sourcetableName.ToLower());

                if (!string.IsNullOrEmpty(desttableName))
                    query = query.Where(x => x.DestTableName.ToLower() == desttableName.ToLower());

                if (!string.IsNullOrEmpty(linkType))
                    query = query.Where(x => x.LinkType.ToLower() == linkType.ToLower());

                if (sourceRowId > 0)
                    query = query.Where(x => x.SourceRowId == sourceRowId);

                if (destRowId > 0)
                    query = query.Where(x => x.DestRowId == destRowId);

                if (creatorId > 0)
                    query = query.Where(x => x.CreatorId == creatorId);


              

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.LinkType) && x.LinkType.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.SourceTableName) && x.SourceTableName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.DestTableName) && x.DestTableName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
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

        public async Task<ListResultObject<object>> GetLinkedObjectsAsync(
string sourcetableName = "", string desttableName = "", long sourceRowId = 0, string linkType = "", 
int pageIndex = 1, int pageSize = 20, string sortQuery = "")
        {
            ListResultObject<object> results = new ListResultObject<object>();

            try
            {
                var getIdsquery = await GetAllLinkedEntitiesAsync(sourcetableName,desttableName,sourceRowId,0,linkType,0,pageIndex,pageSize);

                var linkIds = string.Join(",", getIdsquery.Results.Select(x=> x.DestRowId).ToList());

                var linkIdsExpr = getIdsquery.Results.Count > 0 ? $"and {desttableName}.ID IN ({linkIds})" : $"and {desttableName}.ID IN (0)";

                string sqlQuery = @$"
SELECT  {desttableName}.* ,Images.GetUrl as imageGetUrl FROM {desttableName} 
inner join images
on Images.ForeignKeyId = {desttableName}.ID
WHERE Images.EntityType = '{desttableName.ToLower().Singularize()}' {linkIdsExpr}
and Courses.IsActive = 1 and Images.IsActive = 1
";

                var dt = await _context.ToDataTableAsync(sqlQuery);

                results.TotalCount = dt.Rows.Count;
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = JsonConvert.DeserializeObject<List<object>>(dt.ConvertDtToJson());
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