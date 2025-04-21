using AITechDATA.DataLayer.Repositories;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;

using AITechDATA.Domain;
using System.Text.RegularExpressions;
using Group = AITechDATA.Domain.Group;

namespace AITechDATA.DataLayer.Services
{
    public class GroupRep : IGroupRep
    {
        private AiITechContext _context;

        public GroupRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddGroupAsync(Group group)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Groups.AddAsync(group);
                await _context.SaveChangesAsync();
                result.ID = group.ID;
                _context.Entry(group).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditGroupAsync(Group group)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Groups.Update(group);
                await _context.SaveChangesAsync();
                result.ID = group.ID;
                _context.Entry(group).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistGroupAsync(long groupId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Groups
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == groupId);
                result.ID = groupId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Group>> GetAllGroupsAsync(long userId=0,long courseId = 0, string groupStatus = "", int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Group> results = new ListResultObject<Group>();
            IQueryable<Group> query;
            try
            {
                query = _context.Groups.AsNoTracking()
                                        .Include(x => x.Course)
                    .Include(x => x.Teacher)
                    .Include(x => x.Sessions)
                    .Include(x => x.PreRegistrations)
                    .Include(x => x.Students)
;
                if (userId > 0)
                {
                    query = _context.UserGroups.Where(x => x.UserId == userId).Include(x=> x.Group).ThenInclude(x=> x.Teacher).Select(x => x.Group);
                }
                if (courseId > 0)
                {
                    query = _context.Groups.AsNoTracking().Where(x => x.CourseId == courseId);
                }

                query = query.Where(x =>
                    ((!string.IsNullOrEmpty(groupStatus) && x.Status == (GroupStatus)Enum.Parse(typeof(GroupStatus), groupStatus))
                    )
                    || ((!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                        (x.Teacher.FullName.Contains(searchText)))
                );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
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

        public async Task<RowResultObject<Group>> GetGroupByIdAsync(long groupId)
        {
            RowResultObject<Group> result = new RowResultObject<Group>();
            try
            {
                result.Result = await _context.Groups
                    .AsNoTracking()
                    .Include(x => x.Course)
                    .Include(x => x.Teacher)
                    .Include(x => x.Sessions)
                    .Include(x => x.PreRegistrations)
                    .Include(x => x.Students)
                    .SingleOrDefaultAsync(x => x.ID == groupId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveGroupAsync(Group group)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
                result.ID = group.ID;
                _context.Entry(group).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveGroupAsync(long groupId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var group = await GetGroupByIdAsync(groupId);
                result = await RemoveGroupAsync(group.Result);
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