using AITechDATA.DataLayer.Repositories;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;

namespace AITechDATA.DataLayer.Servisces
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

        public async Task<ListResultObject<Group>> GetAllGroupsAsync(long courseId=0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Group> results = new ListResultObject<Group>();
            try
            {
                var query = _context.Groups
                    .AsNoTracking()
                    .Where(x =>
                        (courseId > 0 && x.CourseId == courseId)
                        || ((!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                        (x.Teacher.FullName.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Course)
                    .Include(x => x.Teacher)
                    .Include(x => x.Sessions)
                    .Include(x => x.PreRegistrations)
                    .Include(x => x.Students)
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