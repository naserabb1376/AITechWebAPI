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
    public class UserGroupRep : IUserGroupRep
    {
        private AiITechContext _context;

        public UserGroupRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddUserGroupsAsync(List<UserGroup> UserGroups)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.UserGroups.AddRangeAsync(UserGroups);
                await _context.SaveChangesAsync();
                result.ID = UserGroups.FirstOrDefault().ID;
                foreach (var UserGroup in UserGroups)
                {
                    _context.Entry(UserGroup).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditUserGroupsAsync(List<UserGroup> UserGroups)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserGroups.UpdateRange(UserGroups);
                await _context.SaveChangesAsync();
                result.ID = UserGroups.FirstOrDefault().ID;
                foreach (var UserGroup in UserGroups)
                {
                    _context.Entry(UserGroup).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistUserGroupAsync(long UserGroupId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.UserGroups
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == UserGroupId);
                result.ID = UserGroupId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<UserGroup>> GetAllUserGroupsAsync(
      long userId = 0,
      long groupId = 0,
      int pageIndex = 1,
      int pageSize = 20,
      string searchText = "",
      string sortQuery = "")
        {
            var results = new ListResultObject<UserGroup>();
            try
            {
                searchText = (searchText ?? string.Empty).Trim();

                // 1) پایه‌ی فیلتر
                var baseQuery = _context.UserGroups.AsNoTracking();

                if (userId > 0)
                    baseQuery = baseQuery.Where(ug => ug.UserId == userId);

                if (groupId > 0)
                    baseQuery = baseQuery.Where(ug => ug.GroupId == groupId);

                // 2) فیلتر جستجو
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    if (userId > 0 && groupId == 0)
                    {
                        // وقتی دنبال گروه‌های یک کاربر می‌گردیم، روی اطلاعات گروه سرچ کن
                        baseQuery = baseQuery.Where(ug =>
                            (ug.Group.Name ?? "").Contains(searchText));
                    }
                    else if (groupId > 0 && userId == 0)
                    {
                        // وقتی دنبال کاربران یک گروه می‌گردیم، روی اطلاعات کاربر سرچ کن
                        baseQuery = baseQuery.Where(ug =>
                            (ug.User.FullName ?? "").Contains(searchText) ||
                            (ug.User.Email ?? "").Contains(searchText));
                    }
                    else
                    {
                        // حالت عمومی: روی هر دو طرف
                        baseQuery = baseQuery.Where(ug =>
                            (ug.User.FullName ?? "").Contains(searchText) ||
                            (ug.Group.Name ?? "").Contains(searchText));
                    }
                }

                // 3) شمارش قبل از Include (ارزان‌تره)
                results.TotalCount = await baseQuery.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                // 4) Include های هدفمند
                IQueryable<UserGroup> withIncludes = baseQuery;
                if (userId > 0 && groupId == 0)
                {
                    withIncludes = withIncludes.Include(ug => ug.Group);
                }
                else if (groupId > 0 && userId == 0)
                {
                    withIncludes = withIncludes.Include(ug => ug.User);
                }
                else
                {
                    withIncludes = withIncludes
                        .Include(ug => ug.User)
                        .Include(ug => ug.Group);
                }

                // 5) مرتب‌سازی
                IOrderedQueryable<UserGroup> ordered;

                if (string.IsNullOrWhiteSpace(sortQuery))
                {
                    if (userId > 0 && groupId == 0)
                        ordered = withIncludes.OrderBy(ug => ug.Group.Name);
                    else if (groupId > 0 && userId == 0)
                        ordered = withIncludes.OrderBy(ug => ug.User.FullName);
                    else
                        ordered = withIncludes.OrderByDescending(ug => ug.ID);
                }
                else
                {
                    // SortBy => IQueryable برمی‌گردونه؛ اگر قابل کست نبود، fallback بده
                    ordered = withIncludes.SortBy(sortQuery) as IOrderedQueryable<UserGroup>
                              ?? withIncludes.OrderByDescending(ug => ug.ID);
                }

                results.Results = await ordered
                    .ToPaging(pageIndex, pageSize)
                    .ToListAsync();

                // 6) صفحه‌بندی و خروجی
                results.Results = await ordered
                    .ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }


        public async Task<RowResultObject<UserGroup>> GetUserGroupByIdAsync(long UserGroupId)
        {
            RowResultObject<UserGroup> result = new RowResultObject<UserGroup>();
            try
            {
                result.Result = await _context.UserGroups
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Group)
                    .SingleOrDefaultAsync(x => x.ID == UserGroupId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserGroupsAsync(List<UserGroup> UserGroups)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserGroups.RemoveRange(UserGroups);
                await _context.SaveChangesAsync();
                result.ID = UserGroups.FirstOrDefault().ID;
                foreach (var UserGroup in UserGroups)
                {
                    _context.Entry(UserGroup).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserGroupsAsync(List<long> UserGroupIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var UserGroupsToRemove = new List<UserGroup>();

                foreach (var UserGroupId in UserGroupIds)
                {
                    var UserGroup = await GetUserGroupByIdAsync(UserGroupId);
                    if (UserGroup.Result != null)
                    {
                        UserGroupsToRemove.Add(UserGroup.Result);
                    }
                }

                if (UserGroupsToRemove.Any())
                {
                    result = await RemoveUserGroupsAsync(UserGroupsToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching UserGroups found to remove.";
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