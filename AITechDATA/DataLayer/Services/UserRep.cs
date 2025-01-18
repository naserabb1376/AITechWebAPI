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

        public async Task<BitResultObject> ExistUserAsync(string fieldValue, string fieldName)
        {
            BitResultObject result = new BitResultObject();
            long userId = 0;
            try
            {
                switch (fieldName.ToLower().Trim())
                {
                    case "id":
                    default:
                        {
                           var theUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x=> x.ID == long.Parse(fieldValue));
                            userId = theUser.ID;
                            break;
                        }
                    case "username":
                        {
                            var theUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == fieldValue);
                            userId = theUser.ID;
                            break;
                        }
                    case "email":
                        {
                            var theUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == fieldValue);
                            userId = theUser.ID;
                            break;
                        }
                }
                result.ID = userId;
                result.Status = userId > 0;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<User>> GetAllUsersAsync(long groupId=0,long courseId=0,long sessionAssignmentId = 0, long sessionId = 0, long AddressId = 0, long RoleId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<User> results = new ListResultObject<User>();
            try
            {
                IQueryable<User> query;
                if (groupId > 0)
                {
                     query = _context.UserGroups.Where(x=> x.GroupId == groupId).Select(x=> x.User)
                    .AsNoTracking()
                    .Where(x =>
                        (AddressId > 0 && x.AddressId == AddressId) ||
                        (RoleId > 0 && x.RoleId == RoleId) ||
                        ((!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Username) && x.Username.Contains(searchText)))
                    );
                }

                else if (courseId > 0)
                {
                     query = _context.UserCourses.Where(x => x.CourseId == courseId).Select(x => x.User)
                    .AsNoTracking()
                    .Where(x =>
                        (AddressId > 0 && x.AddressId == AddressId) ||
                        (RoleId > 0 && x.RoleId == RoleId) ||
                        ((!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Username) && x.Username.Contains(searchText)))
                    );
                }
                else if (courseId > 0)
                {
                    query = _context.Assignments.Where(x => x.SessionAssignmentId == sessionAssignmentId).Select(x => x.User)
                   .AsNoTracking()
                   .Where(x =>
                       (AddressId > 0 && x.AddressId == AddressId) ||
                       (RoleId > 0 && x.RoleId == RoleId) ||
                       ((!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.Username) && x.Username.Contains(searchText)))
                   );
                }
                if (sessionId > 0)
                {
                    query = _context.Attendances.Where(x => x.SessionId == sessionId).Select(x => x.User)
                   .AsNoTracking()
                   .Where(x =>
                       (AddressId > 0 && x.AddressId == AddressId) ||
                       (RoleId > 0 && x.RoleId == RoleId) ||
                       ((!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.Username) && x.Username.Contains(searchText)))
                   );
                }
                else
                {
                     query = _context.Users
                 .AsNoTracking()
                 .Where(x =>
                     (AddressId > 0 && x.AddressId == AddressId) ||
                     (RoleId > 0 && x.RoleId == RoleId) ||
                     ((!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                     (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                     (!string.IsNullOrEmpty(x.Username) && x.Username.Contains(searchText)))
                 );
                }

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