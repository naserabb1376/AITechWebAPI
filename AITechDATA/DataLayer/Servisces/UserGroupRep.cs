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
    public class UserGroupRep : IUserGroupRep
    {
        private AiITechContext _context;

        public UserGroupRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddUserGroupAsync(UserGroup UserGroup)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.UserGroups.AddAsync(UserGroup);
                await _context.SaveChangesAsync();
                result.ID = UserGroup.ID;
                _context.Entry(UserGroup).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditUserGroupAsync(UserGroup UserGroup)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserGroups.Update(UserGroup);
                await _context.SaveChangesAsync();
                result.ID = UserGroup.ID;
                _context.Entry(UserGroup).State = EntityState.Detached;
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

        public async Task<ListResultObject<UserGroup>> GetAllUserGroupsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<UserGroup> results = new ListResultObject<UserGroup>();
            try
            {
                var query = _context.UserGroups
                    .AsNoTracking()
                    .Where(x =>
                        x.User.FullName.ToString().Contains(searchText) ||
                        x.Group.Name.ToString().Contains(searchText)
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ID)
                    .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .Include(x => x.Group)
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

        public async Task<BitResultObject> RemoveUserGroupAsync(UserGroup UserGroup)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserGroups.Remove(UserGroup);
                await _context.SaveChangesAsync();
                result.ID = UserGroup.ID;
                _context.Entry(UserGroup).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserGroupAsync(long UserGroupId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var UserGroup = await GetUserGroupByIdAsync(UserGroupId);
                result = await RemoveUserGroupAsync(UserGroup.Result);
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