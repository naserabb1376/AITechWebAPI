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
    public class DismissalRep : IDismissalRep
    {
        private AITechContext _context;

        public DismissalRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddDismissalAsync(Dismissal Dismissal)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Dismissals.AddAsync(Dismissal);
                await _context.SaveChangesAsync();
                result.ID = Dismissal.ID;
                _context.Entry(Dismissal).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditDismissalAsync(Dismissal Dismissal)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Dismissals.Update(Dismissal);
                await _context.SaveChangesAsync();
                result.ID = Dismissal.ID;
                _context.Entry(Dismissal).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistDismissalAsync(long DismissalId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Dismissals
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == DismissalId);
                result.ID = DismissalId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Dismissal>> GetAllDismissalsAsync(long userId = 0, long checkerUserId = 0, int approveState = 2, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Dismissal> results = new ListResultObject<Dismissal>();
            try
            {
                var query = _context.Dismissals.Include(x=> x.User).Include(x=> x.CheckerUser).AsNoTracking();

                if (userId > 0)
                {
                    query = query.Where(x => x.UserId == userId);
                }

                if (checkerUserId > 0)
                {
                    query = query.Where(x => x.CheckerUserId == checkerUserId);
                }

                if (approveState < 2)
                {
                    query = query.Where(x => x.IsApproved == Convert.ToBoolean(approveState));
                }

                query = query.Where(x =>
                        ((!string.IsNullOrEmpty(x.DismissalType) && x.DismissalType.Contains(searchText)) ||
                        ((!string.IsNullOrEmpty($"{x.User.FirstName} {x.User.LastName}") && $"{x.User.FirstName} {x.User.LastName}".Contains(searchText))) ||
                        ((!string.IsNullOrEmpty($"{x.CheckerUser.FirstName} {x.CheckerUser.LastName}") && $"{x.CheckerUser.FirstName} {x.CheckerUser.LastName}".Contains(searchText))) ||
                        ((!string.IsNullOrEmpty(x.DismissalRequestDescription) && x.DismissalRequestDescription.Contains(searchText)) ||
                        ((!string.IsNullOrEmpty(x.CheckerDescription) && x.CheckerDescription.Contains(searchText))
                        ))));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Dismissal>> GetDismissalByIdAsync(long DismissalId)
        {
            RowResultObject<Dismissal> result = new RowResultObject<Dismissal>();
            try
            {
                result.Result = await _context.Dismissals
                    .AsNoTracking()
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.ID == DismissalId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDismissalAsync(Dismissal Dismissal)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Dismissals.Remove(Dismissal);
                await _context.SaveChangesAsync();
                result.ID = Dismissal.ID;
                _context.Entry(Dismissal).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDismissalAsync(long DismissalId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Dismissal = await GetDismissalByIdAsync(DismissalId);
                result = await RemoveDismissalAsync(Dismissal.Result);
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