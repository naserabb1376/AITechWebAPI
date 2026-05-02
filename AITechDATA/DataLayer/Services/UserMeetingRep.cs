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
    public class UserMeetingRep : IUserMeetingRep
    {
        private AITechContext _context;

        public UserMeetingRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddUserMeetingsAsync(List<UserMeeting> UserMeetings)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.UserMeetings.AddRangeAsync(UserMeetings);
                await _context.SaveChangesAsync();
                result.ID = UserMeetings.FirstOrDefault().ID;
                foreach (var UserMeeting in UserMeetings)
                {
                    _context.Entry(UserMeeting).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditUserMeetingsAsync(List<UserMeeting> UserMeetings)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserMeetings.UpdateRange(UserMeetings);
                await _context.SaveChangesAsync();
                result.ID = UserMeetings.FirstOrDefault().ID;
                foreach (var UserMeeting in UserMeetings)
                {
                    _context.Entry(UserMeeting).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistUserMeetingAsync(long UserMeetingId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.UserMeetings
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == UserMeetingId);
                result.ID = UserMeetingId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<UserMeeting>> GetAllUserMeetingsAsync(
      long userId = 0,
      long meetingId = 0,
      int pageIndex = 1,
      int pageSize = 20,
      string searchText = "",
      string sortQuery = "")
        {
            var results = new ListResultObject<UserMeeting>();
            try
            {
                searchText = (searchText ?? string.Empty).Trim();

                // 1) پایه‌ی فیلتر
                var baseQuery = _context.UserMeetings.AsNoTracking();

                if (userId > 0)
                    baseQuery = baseQuery.Where(ug => ug.UserId == userId);

                if (meetingId > 0)
                    baseQuery = baseQuery.Where(ug => ug.MeetingId == meetingId);

                // 2) فیلتر جستجو
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    if (userId > 0 && meetingId == 0)
                    {
                        // وقتی دنبال جلسه‌های یک کاربر می‌گردیم، روی اطلاعات جلسه سرچ کن
                        baseQuery = baseQuery.Where(ug =>
                            (ug.Meeting.MeetingTitle ?? "").Contains(searchText)
                           || (ug.Meeting.MeetingStatus ?? "").Contains(searchText)
                            || (ug.Meeting.MeetingOrganizer ?? "").Contains(searchText)
                            );
                    }
                    else if (meetingId > 0 && userId == 0)
                    {
                        // وقتی دنبال کاربران یک جلسه می‌گردیم، روی اطلاعات کاربر سرچ کن
                        baseQuery = baseQuery.Where(x =>
                       (!string.IsNullOrEmpty(x.User.FirstName) && x.User.FirstName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.User.LastName) && x.User.LastName.Contains(searchText)) ||
                            (x.User.Email ?? "").Contains(searchText));
                    }
                    else
                    {
                        // حالت عمومی: روی هر دو طرف
                        baseQuery = baseQuery.Where(x =>
                       (!string.IsNullOrEmpty(x.User.FirstName) && x.User.FirstName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.User.LastName) && x.User.LastName.Contains(searchText)) ||
                            (x.Meeting.MeetingTitle ?? "").Contains(searchText)
                           || (x.Meeting.MeetingStatus ?? "").Contains(searchText)
                            || (x.Meeting.MeetingOrganizer ?? "").Contains(searchText)
                            );
                    }
                }

                // 3) شمارش قبل از Include (ارزان‌تره)
                results.TotalCount = await baseQuery.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                // 4) Include های هدفمند
                IQueryable<UserMeeting> withIncludes = baseQuery;
                withIncludes = withIncludes
                        .Include(ug => ug.User)
                        .Include(ug => ug.Meeting);

                // 5) مرتب‌سازی
                IOrderedQueryable<UserMeeting> ordered;

                if (string.IsNullOrWhiteSpace(sortQuery))
                {
                    if (userId > 0 && meetingId == 0)
                        ordered = withIncludes.OrderBy(ug => ug.Meeting.MeetingTitle);
                    else if (meetingId > 0 && userId == 0)
                        ordered = withIncludes.OrderBy(ug => ug.User.LastName);
                    else
                        ordered = withIncludes.OrderByDescending(ug => ug.ID);
                }
                else
                {
                    // SortBy => IQueryable برمی‌گردونه؛ اگر قابل کست نبود، fallback بده
                    ordered = withIncludes.SortBy(sortQuery) as IOrderedQueryable<UserMeeting>
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


        public async Task<RowResultObject<UserMeeting>> GetUserMeetingByIdAsync(long UserMeetingId)
        {
            RowResultObject<UserMeeting> result = new RowResultObject<UserMeeting>();
            try
            {
                result.Result = await _context.UserMeetings
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Meeting)
                    .SingleOrDefaultAsync(x => x.ID == UserMeetingId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserMeetingsAsync(List<UserMeeting> UserMeetings)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.UserMeetings.RemoveRange(UserMeetings);
                await _context.SaveChangesAsync();
                result.ID = UserMeetings.FirstOrDefault().ID;
                foreach (var UserMeeting in UserMeetings)
                {
                    _context.Entry(UserMeeting).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveUserMeetingsAsync(List<long> UserMeetingIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var UserMeetingsToRemove = new List<UserMeeting>();

                foreach (var UserMeetingId in UserMeetingIds)
                {
                    var UserMeeting = await GetUserMeetingByIdAsync(UserMeetingId);
                    if (UserMeeting.Result != null)
                    {
                        UserMeetingsToRemove.Add(UserMeeting.Result);
                    }
                }

                if (UserMeetingsToRemove.Any())
                {
                    result = await RemoveUserMeetingsAsync(UserMeetingsToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching UserMeetings found to remove.";
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