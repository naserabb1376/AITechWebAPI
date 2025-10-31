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
    public class AwardRep : IAwardRep
    {
        private AITechContext _context;

        public AwardRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddAwardAsync(Award Award)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Awards.AddAsync(Award);
                await _context.SaveChangesAsync();
                result.ID = Award.ID;
                _context.Entry(Award).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

      

        public async Task<BitResultObject> EditAwardAsync(Award Award)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Awards.Update(Award);
                await _context.SaveChangesAsync();
                result.ID = Award.ID;
                _context.Entry(Award).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistAwardAsync(long AwardId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Awards
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == AwardId);
                result.ID = AwardId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Award>> GetAllAwardsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Award> results = new ListResultObject<Award>();
            try
            {
                var query = _context.Awards
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.AwardTitle) && x.AwardTitle.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
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

        public async Task<RowResultObject<Award>> GetAwardByIdAsync(long AwardId)
        {
            RowResultObject<Award> result = new RowResultObject<Award>();
            try
            {
                result.Result = await _context.Awards
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == AwardId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAwardAsync(Award Award)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Awards.Remove(Award);
                await _context.SaveChangesAsync();
                result.ID = Award.ID;
                _context.Entry(Award).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAwardAsync(long AwardId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Award = await GetAwardByIdAsync(AwardId);
                result = await RemoveAwardAsync(Award.Result);
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