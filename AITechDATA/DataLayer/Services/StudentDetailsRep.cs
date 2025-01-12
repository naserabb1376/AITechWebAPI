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
    public class StudentDetailsRep : IStudentDetailsRep
    {
        private AiITechContext _context;

        public StudentDetailsRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddStudentDetailsAsync(StudentDetails studentDetails)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.StudentDetails.AddAsync(studentDetails);
                await _context.SaveChangesAsync();
                result.ID = studentDetails.ID;
                _context.Entry(studentDetails).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditStudentDetailsAsync(StudentDetails studentDetails)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.StudentDetails.Update(studentDetails);
                await _context.SaveChangesAsync();
                result.ID = studentDetails.ID;
                _context.Entry(studentDetails).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistStudentDetailsAsync(long studentDetailsId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.StudentDetails
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == studentDetailsId);
                result.ID = studentDetailsId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<StudentDetails>> GetAllStudentDetailsAsync(long UserId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<StudentDetails> results = new ListResultObject<StudentDetails>();
            try
            {
                var query = _context.StudentDetails
                    .AsNoTracking()
                    .Where(x =>
                        (UserId > 0 && x.UserId == UserId) ||
                        (!string.IsNullOrEmpty(x.User.FullName) && x.User.FullName.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User).ThenInclude(x => x.Address)
                    .Include(x => x.Parents)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<StudentDetails>> GetStudentDetailsByIdAsync(long studentDetailsId)
        {
            RowResultObject<StudentDetails> result = new RowResultObject<StudentDetails>();
            try
            {
                result.Result = await _context.StudentDetails
                    .AsNoTracking()
                    .Include(x => x.User).ThenInclude(x => x.Address)
                    .Include(x => x.Parents)
                    .SingleOrDefaultAsync(x => x.ID == studentDetailsId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveStudentDetailsAsync(StudentDetails studentDetails)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.StudentDetails.Remove(studentDetails);
                await _context.SaveChangesAsync();
                result.ID = studentDetails.ID;
                _context.Entry(studentDetails).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveStudentDetailsAsync(long studentDetailsId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var studentDetails = await GetStudentDetailsByIdAsync(studentDetailsId);
                result = await RemoveStudentDetailsAsync(studentDetails.Result);
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