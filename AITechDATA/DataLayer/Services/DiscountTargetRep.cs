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
using System.Runtime.ConstrainedExecution;
using MTPermissionCenter.EFCore.Entities;

namespace AITechDATA.DataLayer.Services
{
    public class DiscountTargetRep : IDiscountTargetRep
    {
        private AITechContext _context;

        public DiscountTargetRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddDiscountTargetsAsync(List<DiscountTarget> DiscountTargets)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                DiscountTargets = DiscountTargets.Where(p=>  ! _context.DiscountTargets.Any(x=> x.DiscountId == p.DiscountId && x.TargetId == p.TargetId)).ToList();
                await _context.DiscountTargets.AddRangeAsync(DiscountTargets);
                await _context.SaveChangesAsync();
                result.ID = DiscountTargets.Count> 0 ?  DiscountTargets.FirstOrDefault().ID : 0;
                foreach (var DiscountTarget in DiscountTargets)
                {
                    _context.Entry(DiscountTarget).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditDiscountTargetsAsync(List<DiscountTarget> DiscountTargets)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                DiscountTargets = DiscountTargets.Where(p => !_context.DiscountTargets.Any(x => x.DiscountId == p.DiscountId && x.TargetId == p.TargetId)).ToList();
                _context.DiscountTargets.UpdateRange(DiscountTargets);
                await _context.SaveChangesAsync();
                result.ID = DiscountTargets.Count > 0 ? DiscountTargets.FirstOrDefault().ID : 0;
                foreach (var DiscountTarget in DiscountTargets)
                {
                    _context.Entry(DiscountTarget).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistDiscountTargetAsync(long DiscountTargetId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.DiscountTargets
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == DiscountTargetId);
                result.ID = DiscountTargetId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<DiscountTarget>> GetAllDiscountTargetsAsync(long DiscountId = 0, long TargetId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<DiscountTarget> results = new ListResultObject<DiscountTarget>();
            try
            {
                var query = _context.DiscountTargets.Include(x => x.DiscountId).AsNoTracking();

                if (DiscountId > 0)
                {
                    query = query.Where(x => x.DiscountId == DiscountId);
                }

                query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Discount.DiscountCode) && x.Discount.DiscountCode.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Discount.EntityName) && x.Discount.EntityName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.TargetEntityName) && x.TargetEntityName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Discount.Description) && x.Discount.Description.Contains(searchText))
                    );


                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ID)
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

        public async Task<RowResultObject<DiscountTarget>> GetDiscountTargetByIdAsync(long DiscountTargetId)
        {
            RowResultObject<DiscountTarget> result = new RowResultObject<DiscountTarget>();
            try
            {
                result.Result = await _context.DiscountTargets
                    .AsNoTracking()
                    .Include(x => x.Discount)
                    .SingleOrDefaultAsync(x => x.ID == DiscountTargetId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDiscountTargetsAsync(List<DiscountTarget> DiscountTargets)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.DiscountTargets.RemoveRange(DiscountTargets);
                await _context.SaveChangesAsync();
                result.ID = DiscountTargets.FirstOrDefault().ID;
                foreach (var DiscountTarget in DiscountTargets)
                {
                    _context.Entry(DiscountTarget).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDiscountTargetsAsync(List<long> DiscountTargetIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var DiscountTargetsToRemove = new List<DiscountTarget>();

                foreach (var DiscountTargetId in DiscountTargetIds)
                {
                    var DiscountTarget = await GetDiscountTargetByIdAsync(DiscountTargetId);
                    if (DiscountTarget.Result != null)
                    {
                        DiscountTargetsToRemove.Add(DiscountTarget.Result);
                    }
                }

                if (DiscountTargetsToRemove.Any())
                {
                    result = await RemoveDiscountTargetsAsync(DiscountTargetsToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching DiscountTargets found to remove.";
                }
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