using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using NobatPlusDATA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Services
{
    public class GroupChatReadStateRep : IGroupChatReadStateRep
    {

        private AITechContext _context;
        public GroupChatReadStateRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddGroupChatReadStateAsync(GroupChatReadState GroupChatReadState)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.GroupChatReadStates.AddAsync(GroupChatReadState);
                await _context.SaveChangesAsync();
                result.ID = GroupChatReadState.ID;
                _context.Entry(GroupChatReadState).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> EditGroupChatReadStateAsync(GroupChatReadState GroupChatReadState)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.GroupChatReadStates.Update(GroupChatReadState);
                await _context.SaveChangesAsync();
                result.ID = GroupChatReadState.ID;
                _context.Entry(GroupChatReadState).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> ExistGroupChatReadStateAsync(long GroupChatReadStateId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.GroupChatReadStates
                .AsNoTracking()
                .AnyAsync(x => x.ID == GroupChatReadStateId);
                result.ID = GroupChatReadStateId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<ListResultObject<GroupChatReadState>> GetAllGroupChatReadStatesAsync(long GroupId = 0, long UserId = 0,long RoleId=0, long LastReadMessageId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<GroupChatReadState> results = new ListResultObject<GroupChatReadState>();
            try
            {
                if (GroupId > 0 && UserId > 0)
                {
                    var canAccess = await CanAccessGroupChatAsync(GroupId, UserId,RoleId);
                    if (!canAccess)
                    {
                        results.ErrorMessage = "شما به این گروه چت دسترسی ندارید";
                        results.Status = false;
                        return results;
                    }

                }


                IQueryable<GroupChatReadState> query = _context.GroupChatReadStates
                        .AsNoTracking().Include(x => x.User).Include(x => x.Group);


                if (UserId > 0)
                {
                    query = query.Where(x=> x.UserId == UserId);
                }

                if (GroupId > 0)
                {
                    query = query.Where(x => x.GroupId == GroupId);
                }

                if (LastReadMessageId > 0)
                {
                    query = query.Where(x => x.LastReadMessageId == LastReadMessageId);
                }

                query = query.Where(x =>
                            (!string.IsNullOrEmpty(x.Group.Name) && x.Group.Name.Contains(searchText)) ||
                            (!string.IsNullOrEmpty(x.User.FirstName) && x.User.FirstName.Contains(searchText)) ||
                            (!string.IsNullOrEmpty(x.User.LastName) && x.User.LastName.Contains(searchText)) ||
                            (x.LastReadAt.HasValue && x.LastReadAt.Value.ToString().Contains(searchText)) ||
                            (x.CreateDate.HasValue && x.CreateDate.Value.ToString().Contains(searchText)) ||
                            (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString().Contains(searchText))
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

        public async Task<RowResultObject<GroupChatReadState>> GetGroupChatReadStateByIdAsync(long GroupChatReadStateId)
        {
            RowResultObject<GroupChatReadState> result = new RowResultObject<GroupChatReadState>();
            try
            {
                result.Result = await _context.GroupChatReadStates
                .AsNoTracking().Include(x => x.User).Include(x => x.Group).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> RemoveGroupChatReadStateAsync(GroupChatReadState GroupChatReadState)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.GroupChatReadStates.Remove(GroupChatReadState);
                await _context.SaveChangesAsync();
                result.ID = GroupChatReadState.ID;
                _context.Entry(GroupChatReadState).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> RemoveGroupChatReadStateAsync(long GroupChatReadStateId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var GroupChatReadState = await GetGroupChatReadStateByIdAsync(GroupChatReadStateId);
                result = await RemoveGroupChatReadStateAsync(GroupChatReadState.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }


        // Messenger

        public async Task<bool> CanAccessGroupChatAsync(long groupId, long userId, long roleId)
        {
            var existGroup = await _context.Groups
               .AsNoTracking()
               .AnyAsync(g => g.ID == groupId);
            if (!existGroup) return false;

            // ادمین
            var isAdmin = roleId == 3 || roleId == 4 || roleId == 7;

            if (isAdmin) return true;

            // استاد گروه
            var isTeacher = await _context.Groups
                .AsNoTracking()
                .AnyAsync(g => g.ID == groupId && g.TeacherId == userId);

            if (isTeacher) return true;

            // دانش‌آموز عضو گروه (UserGroup)
            var isStudent = await _context.UserGroups
                .AsNoTracking()
                .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

            return isStudent;
        }



        public async Task MarkAsSeenAsync(long groupId, long userId,long roleId, long lastReadMessageId)
        {
            var canAccess = await CanAccessGroupChatAsync(groupId, userId,roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            // پیام باید متعلق به همین گروه باشد
            var exists = await _context.GroupChatMessages.AsNoTracking()
                .AnyAsync(m => m.GroupId == groupId && m.ID == lastReadMessageId && !m.IsDeleted);

            if (!exists)
                throw new KeyNotFoundException("پیام مورد نظر برای ثبت وضعیت مشاهده پیدا نشد");

            var state = await _context.GroupChatReadStates
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);

            if (state == null)
            {
                state = new GroupChatReadState
                {
                    GroupId = groupId,
                    UserId = userId,
                    LastReadMessageId = lastReadMessageId,
                    LastReadAt = DateTime.UtcNow
                };
                _context.GroupChatReadStates.Add(state);
            }
            else
            {
                // فقط رو به جلو
                if (state.LastReadMessageId == null || lastReadMessageId > state.LastReadMessageId.Value)
                {
                    state.LastReadMessageId = lastReadMessageId;
                    state.LastReadAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task<GroupChatReadStateDto> GetReadStateAsync(long groupId, long userId,long roleId)
        {
            var canAccess = await CanAccessGroupChatAsync(groupId, userId,roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            var state = await _context.GroupChatReadStates
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);

            if (state == null)
            {
                return new GroupChatReadStateDto
                {
                    GroupId = groupId,
                    UserId = userId,
                    LastReadMessageId = null,
                    LastReadAt = null
                };
            }

            return new GroupChatReadStateDto
            {
                GroupId = state.GroupId,
                UserId = state.UserId,
                LastReadMessageId = state.LastReadMessageId,
                LastReadAt = state.LastReadAt
            };
        }

        public async Task<List<GroupMemberReadStateDto>> GetGroupReadStatesAsync(long groupId, long userId,long roleId)
        {
            var canAccess = await CanAccessGroupChatAsync(groupId, userId, roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            // اعضای گروه = استاد + دانشجویان (UserGroup)
            // ما اینجا همه رو خروجی می‌دیم. (حتی اگر هنوز Seen ندارن)
            var teacherId = await _context.Groups.AsNoTracking()
                .Where(g => g.ID == groupId)
                .Select(g => g.TeacherId)
                .FirstOrDefaultAsync();

            if (teacherId == 0)
                throw new KeyNotFoundException("گروه پیدا نشد");

            // لیست دانشجویان
            var studentsQuery =
                from ug in _context.UserGroups.AsNoTracking()
                where ug.GroupId == groupId
                join u in _context.Users.AsNoTracking() on ug.UserId equals u.ID
                select new { u.ID, FullName = (u.FirstName + " " + u.LastName) };

            // استاد
            var teacherQuery =
                from u in _context.Users.AsNoTracking()
                where u.ID == teacherId
                select new { u.ID, FullName = (u.FirstName + " " + u.LastName) };

            var members = await teacherQuery
                .Union(studentsQuery)
                .ToListAsync();

            // وضعیت Seen
            var readStates = await _context.GroupChatReadStates.AsNoTracking()
                .Where(x => x.GroupId == groupId)
                .ToListAsync();

            // Merge
            var result = members
                .Select(m =>
                {
                    var s = readStates.FirstOrDefault(x => x.UserId == m.ID);
                    return new GroupMemberReadStateDto
                    {
                        UserId = m.ID,
                        FullName = m.FullName,
                        LastReadMessageId = s?.LastReadMessageId,
                        LastReadAt = s?.LastReadAt
                    };
                })
                .OrderByDescending(x => x.LastReadAt) // اختیاری: مرتب‌سازی
                .ToList();

            return result;
        }


    }
}
