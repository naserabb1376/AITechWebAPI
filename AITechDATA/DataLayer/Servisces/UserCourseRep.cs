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
    public class UserCourseRep : IUserCourseRep
    {
        private AiITechContext _context;

        public UserCourseRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddUserCourseAsync(UserCourse UserCourse)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.UserCourses.AddAsync(UserCourse);
                await _context.SaveChangesAsync();
                result.ID = UserCourse.ID;
                _context.Entry(UserCourse).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditUserCourseAsync(UserCourse UserCourse)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserCourses.Update(UserCourse);
                await _context.SaveChangesAsync();
                result.ID = UserCourse.ID;
                _context.Entry(UserCourse).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistUserCourseAsync(long UserCourseId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.UserCourses
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == UserCourseId);
                result.ID = UserCourseId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<UserCourse>> GetAllUserCoursesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<UserCourse> results = new ListResultObject<UserCourse>();
            try
            {
                var query = _context.UserCourses
                    .AsNoTracking()
                    .Where(x =>
                        x.User.FullName.ToString().Contains(searchText) ||
                        x.Course.Title.ToString().Contains(searchText)
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ID)
                    .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .Include(x => x.Course)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<UserCourse>> GetUserCourseByIdAsync(long UserCourseId)
        {
            RowResultObject<UserCourse> result = new RowResultObject<UserCourse>();
            try
            {
                result.Result = await _context.UserCourses
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Course)
                    .SingleOrDefaultAsync(x => x.ID == UserCourseId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserCourseAsync(UserCourse UserCourse)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserCourses.Remove(UserCourse);
                await _context.SaveChangesAsync();
                result.ID = UserCourse.ID;
                _context.Entry(UserCourse).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserCourseAsync(long UserCourseId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var UserCourse = await GetUserCourseByIdAsync(UserCourseId);
                result = await RemoveUserCourseAsync(UserCourse.Result);
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