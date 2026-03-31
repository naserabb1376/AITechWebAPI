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
        private AITechContext _context;

        public GroupRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddGroupAsync(Group group)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                group.RegisterCount = 0;
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
                var groupusers = await _context.UserGroups.Where(x => x.GroupId == group.ID).ToListAsync();
                var groupprs = await _context.PreRegistrations.Where(x => x.EntityType.ToLower() == "group" && x.ForeignKeyId == group.ID).ToListAsync();

                group.RegisterCount = groupusers.Count + groupprs.Count;

                if (group.GroupCapacity < group.RegisterCount)
                {
                    throw new Exception($"تعداد ثبت نام انجام شده در این گروه بیش از {group.GroupCapacity} است");
                }

                if (group.Status == GroupStatus.Active)
                {
                    var newUserGroups = new List<UserGroup>();

                    foreach ( var pr in groupprs )
                    {
                        var existuser = await _context.Users.FirstOrDefaultAsync(u => u.Username == pr.PhoneNumber);
                        if ( existuser != null && !groupusers.Any(x=> x.UserId == existuser.ID))
                        {
                            var newUsergroup = new UserGroup()
                            {
                                CreateDate = DateTime.Now.ToShamsi(),
                                UpdateDate = DateTime.Now.ToShamsi(),
                                IsActive = true,
                                OtherLangs = "",

                                GroupId = group.ID,
                                UserId = existuser.ID,
                                
                            };

                            newUserGroups.Add(newUsergroup);
                        }
                    }

                    await _context.UserGroups.AddRangeAsync(newUserGroups);

                    group.GroupCapacity -= newUserGroups.Count;
                }
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

        public async Task<ListResultObject<GroupDto>> GetAllGroupsAsync(
            long ClientUserId,
            long ClientRoleId,
            long studentId = 0,
            long courseId = 0,
            long teacherId = 0,
            string groupStatus = "",
            string groupType = "",
            int pageIndex = 1,
            int pageSize = 20,
            string searchText = "",
            string sortQuery = "")
        {
            var results = new ListResultObject<GroupDto>();

            try
            {
                // شروع کوئری اصلی با includeهای کامل
                IQueryable<Group> query = _context.Groups
                    .AsNoTracking()
                    .Include(x => x.Course)
                    .Include(x => x.Teacher)
                    .Include(x => x.Sessions)
                   // .Include(x => x.PreRegistrations)
                    .Include(x => x.Students);

                // اگر studentId داده شده، فقط گروه‌های مرتبط با آن کاربر
                if (studentId > 0)
                {
                    query = _context.UserGroups
                        .Where(x => x.UserId == studentId)
                        .Include(x => x.Group).ThenInclude(g => g.Course)
                        .Include(x => x.Group).ThenInclude(g => g.Teacher)
                        .Select(x => x.Group)
                        .AsQueryable();
                }

                // فیلترهای شرطی
                if (courseId > 0)
                {
                    query = query.Where(x => x.CourseId == courseId);
                }

                if (teacherId > 0)
                {
                    query = query.Where(x => x.TeacherId == teacherId);
                }

                if (!string.IsNullOrEmpty(groupStatus) &&
                    Enum.TryParse<GroupStatus>(groupStatus, out var parsedStatus))
                {
                    query = query.Where(x => x.Status == parsedStatus);
                }

                if (!string.IsNullOrEmpty(groupType))
                {
                    query = query.Where(x => x.GroupType.ToLower() == groupType.ToLower());
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.GroupType) && x.GroupType.Contains(searchText)) ||
                       (x.Teacher != null && (!string.IsNullOrEmpty(x.Teacher.FirstName) && x.Teacher.FirstName.Contains(searchText))) ||
                       (x.Teacher != null && (!string.IsNullOrEmpty(x.Teacher.LastName) && x.Teacher.LastName.Contains(searchText)))
                       );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.Select(x=> new GroupDto()
                {
                    ChatMessages = x.ChatMessages,
                    Course = x.Course,
                    CourseId = x.CourseId,
                    PaymentHistories = x.PaymentHistories,
                    Sessions = x.Sessions,
                    Students = x.Students,
                    Teacher = x.Teacher,

                    CreateDate = x.CreateDate,
                    UpdateDate = x.UpdateDate,
                    OtherLangs = x.OtherLangs,
                    IsActive = x.IsActive,
                    ID = x.ID,

                    TeacherId = x.TeacherId,
                    Status = x.Status,
                    Note = x.Note,
                    DayOfWeek = x.DayOfWeek,
                    EndDate = x.EndDate,
                    EndTime = x.EndTime,
                    GroupType = x.GroupType,
                    GroupCapacity = x.GroupCapacity,
                    RegisterCount = x.RegisterCount,
                    StartDate = x.StartDate,
                    StartTime = x.StartTime,
                    Fee = x.Fee,
                    Name = x.Name,

                    DiscountPercent = _context.GetDiscount(x.Fee, "group", x.ID, ClientUserId, ClientRoleId).DiscountPercent,
                    DiscountedFee = _context.GetDiscount(x.Fee, "group", x.ID, ClientUserId, ClientRoleId).DiscountedFee

                })
                    .OrderByDescending(x => x.CreateDate)
                    .SortBy(sortQuery)
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


        public async Task<RowResultObject<GroupDto>> GetGroupByIdAsync(long groupId,long ClientUserId = 0, long ClientRoleId = 0)
        {
            RowResultObject<GroupDto> result = new RowResultObject<GroupDto>();
            try
            {
                result.Result = await _context.Groups
                    .AsNoTracking()
                    .Include(x => x.Course)
                    .Include(x => x.Teacher)
                    .Include(x => x.Sessions)
                    .Include(x => x.Students)
                    .Select(x => new GroupDto()
                    {
                        ChatMessages = x.ChatMessages,
                        Course = x.Course,
                        CourseId = x.CourseId,
                        PaymentHistories = x.PaymentHistories,
                        Sessions = x.Sessions,
                        Students = x.Students,
                        Teacher = x.Teacher,

                        CreateDate = x.CreateDate,
                        UpdateDate = x.UpdateDate,
                        OtherLangs = x.OtherLangs,
                        IsActive = x.IsActive,
                        ID = x.ID,

                        TeacherId = x.TeacherId,
                        Status = x.Status,
                        Note = x.Note,
                        DayOfWeek = x.DayOfWeek,
                        EndDate = x.EndDate,
                        EndTime = x.EndTime,
                        GroupType = x.GroupType,
                        StartDate = x.StartDate,
                        StartTime = x.StartTime,
                        Fee = x.Fee,
                        Name = x.Name,
                        GroupCapacity = x.GroupCapacity,
                        RegisterCount = x.RegisterCount,

                        DiscountPercent = _context.GetDiscount(x.Fee, "group", x.ID, ClientUserId, ClientRoleId).DiscountPercent,
                        DiscountedFee = _context.GetDiscount(x.Fee, "group", x.ID, ClientUserId, ClientRoleId).DiscountedFee

                    })
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
                var x = group.Result;
                var thegroup =new Group()
                {
                    ChatMessages = x.ChatMessages,
                    Course = x.Course,
                    CourseId = x.CourseId,
                    PaymentHistories = x.PaymentHistories,
                    Sessions = x.Sessions,
                    Students = x.Students,
                    Teacher = x.Teacher,

                    CreateDate = x.CreateDate,
                    UpdateDate = x.UpdateDate,
                    OtherLangs = x.OtherLangs,
                    IsActive = x.IsActive,
                    ID = x.ID,

                    TeacherId = x.TeacherId,
                    Status = x.Status,
                    Note = x.Note,
                    DayOfWeek = x.DayOfWeek,
                    EndDate = x.EndDate,
                    EndTime = x.EndTime,
                    GroupType = x.GroupType,
                    StartDate = x.StartDate,
                    StartTime = x.StartTime,
                    Fee = x.Fee,
                    Name = x.Name,

                };
                result = await RemoveGroupAsync(thegroup);
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