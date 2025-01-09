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

namespace AITechDATA.DataLayer.Servisces
{
    public class ParentRep : IParentRep
    {
        private AiITechContext _context;

        public ParentRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddParentAsync(Parent parent)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Parents.AddAsync(parent);
                await _context.SaveChangesAsync();
                result.ID = parent.ID;
                _context.Entry(parent).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditParentAsync(Parent parent)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Parents.Update(parent);
                await _context.SaveChangesAsync();
                result.ID = parent.ID;
                _context.Entry(parent).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistParentAsync(long parentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Parents
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == parentId);
                result.ID = parentId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Parent>> GetAllParentsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "")
        {
            ListResultObject<Parent> results = new ListResultObject<Parent>();
            try
            {
                var query = _context.Parents
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Job) && x.Job.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.ContactNumber) && x.ContactNumber.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                    .ToPaging(pageIndex, pageSize)
                    .Include(x => x.StudentDetails)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Parent>> GetParentByIdAsync(long parentId)
        {
            RowResultObject<Parent> result = new RowResultObject<Parent>();
            try
            {
                result.Result = await _context.Parents
                    .AsNoTracking()
                    .Include(x => x.StudentDetails)
                    .SingleOrDefaultAsync(x => x.ID == parentId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveParentAsync(Parent parent)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Parents.Remove(parent);
                await _context.SaveChangesAsync();
                result.ID = parent.ID;
                _context.Entry(parent).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveParentAsync(long parentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var parent = await GetParentByIdAsync(parentId);
                result = await RemoveParentAsync(parent.Result);
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