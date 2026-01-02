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

namespace AITechDATA.DataLayer.Services
{
    public class EducationalBackgroundRep : IEducationalBackgroundRep
    {
        private AITechContext _context;

        public EducationalBackgroundRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddEducationalBackgroundAsync(EducationalBackground EducationalBackground)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.EducationalBackgrounds.AddAsync(EducationalBackground);
                await _context.SaveChangesAsync();
                result.ID = EducationalBackground.ID;
                _context.Entry(EducationalBackground).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditEducationalBackgroundAsync(EducationalBackground EducationalBackground)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.EducationalBackgrounds.Update(EducationalBackground);
                await _context.SaveChangesAsync();
                result.ID = EducationalBackground.ID;
                _context.Entry(EducationalBackground).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistEducationalBackgroundAsync(long EducationalBackgroundId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.EducationalBackgrounds
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == EducationalBackgroundId);
                result.ID = EducationalBackgroundId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<EducationalBackground>> GetAllEducationalBackgroundsAsync(long UserId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<EducationalBackground> results = new ListResultObject<EducationalBackground>();
            try
            {
                var query = _context.EducationalBackgrounds.Include(x=> x.User).AsNoTracking();

                if (UserId>0)
                {
                    query = query.Where(x => x.UserId == UserId);
                }
               
                 query = query.Where(x =>
                       (!string.IsNullOrEmpty(x.User.FirstName) && x.User.FirstName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.User.LastName) && x.User.LastName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.EducationalGrade) && x.EducationalGrade.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.StudyField) && x.StudyField.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User).ThenInclude(x => x.Address)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<EducationalBackground>> GetEducationalBackgroundByIdAsync(long EducationalBackgroundId)
        {
            RowResultObject<EducationalBackground> result = new RowResultObject<EducationalBackground>();
            try
            {
                result.Result = await _context.EducationalBackgrounds
                    .AsNoTracking()
                    .Include(x => x.User).ThenInclude(x => x.Address)
                    .SingleOrDefaultAsync(x => x.ID == EducationalBackgroundId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveEducationalBackgroundAsync(EducationalBackground EducationalBackground)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.EducationalBackgrounds.Remove(EducationalBackground);
                await _context.SaveChangesAsync();
                result.ID = EducationalBackground.ID;
                _context.Entry(EducationalBackground).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveEducationalBackgroundAsync(long EducationalBackgroundId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var EducationalBackground = await GetEducationalBackgroundByIdAsync(EducationalBackgroundId);
                result = await RemoveEducationalBackgroundAsync(EducationalBackground.Result);
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