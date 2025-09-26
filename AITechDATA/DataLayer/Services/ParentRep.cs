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
    public class ParentRep : IParentRep
    {
        private AITechContext _context;

        public ParentRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddParentAsync(Parent parent)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var theStudentParents = await GetAllParentsAsync(parent.StudentDetailsId,1,0);

                if (theStudentParents != null && theStudentParents.Results.Count >= 2)
                {
                    throw new Exception("شما مجاز به افزودن بیش از 2 والد برای یک دانش آموز نیستید");
                }

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

        public async Task<ListResultObject<Parent>> GetAllParentsAsync(long StudentDetailsId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Parent> results = new ListResultObject<Parent>();
            try
            {
                var query = _context.Parents.Include(x=> x.StudentDetails).ThenInclude(x=> x.User).AsNoTracking();
                if(StudentDetailsId > 0)
                {
                    query = query.Where(x=> x.StudentDetailsId == StudentDetailsId);
                }
                query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.StudentDetails.User.FirstName) && x.StudentDetails.User.FirstName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.StudentDetails.User.LastName) &&  x.StudentDetails.User.LastName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Job) && x.Job.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.ContactNumber) && x.ContactNumber.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.StudentDetails).ThenInclude(x => x.User)
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
                    .Include(x => x.StudentDetails).ThenInclude(x => x.User)
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