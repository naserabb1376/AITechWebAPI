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
    public class UserRep : IUserRep
    {
        private AiITechContext _context;

        public UserRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddUserAsync(User user)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                result.ID = user.ID;
                _context.Entry(user).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditUserAsync(User user)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                result.ID = user.ID;
                _context.Entry(user).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistUserAsync(long userId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == userId);
                result.ID = userId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<User>> GetAllUsersAsync(long AddressId = 0, long RoleId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<User> results = new ListResultObject<User>();
            try
            {
                var query = _context.Users
                    .AsNoTracking()
                    .Where(x =>
                        (AddressId > 0 && x.AddressId == AddressId) ||
                        (RoleId > 0 && x.RoleId == RoleId) ||
                        ((!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Username) && x.Username.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Role)
                    .Include(x => x.TeacherResumes)
                    .Include(x => x.PaymentHistories)
                    .Include(x => x.UserCourses)
                    // .Include(x => x.CoursesEnrolled)
                    .Include(x => x.Assignments)
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

        public async Task<RowResultObject<User>> GetUserByIdAsync(long userId)
        {
            RowResultObject<User> result = new RowResultObject<User>();
            try
            {
                result.Result = await _context.Users
                    .AsNoTracking()
                    .Include(x => x.Role)
                    .Include(x => x.TeacherResumes)
                    .Include(x => x.PaymentHistories)
                    .Include(x => x.UserCourses)
                    //.Include(x => x.CoursesEnrolled)
                    .Include(x => x.Assignments)
                    .Include(x => x.StudentDetails)
                    .SingleOrDefaultAsync(x => x.ID == userId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserAsync(User user)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                result.ID = user.ID;
                _context.Entry(user).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserAsync(long userId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var user = await GetUserByIdAsync(userId);
                result = await RemoveUserAsync(user.Result);
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