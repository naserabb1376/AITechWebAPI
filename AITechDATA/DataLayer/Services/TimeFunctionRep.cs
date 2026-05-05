using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace AITechDATA.DataLayer.Services
{
    public class TimeFunctionRep : ITimeFunctionRep
    {
        private AITechContext _context;

        public TimeFunctionRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddTimeFunctionAsync(TimeFunction TimeFunction)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.TimeFunctions.AddAsync(TimeFunction);
                await _context.SaveChangesAsync();
                result.ID = TimeFunction.ID;
                _context.Entry(TimeFunction).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditTimeFunctionAsync(TimeFunction TimeFunction)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TimeFunctions.Update(TimeFunction);
                await _context.SaveChangesAsync();
                result.ID = TimeFunction.ID;
                _context.Entry(TimeFunction).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistTimeFunctionAsync(long TimeFunctionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.TimeFunctions
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == TimeFunctionId);
                result.ID = TimeFunctionId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<TimeFunctionDto>> GetAllTimeFunctionsAsync(long userId = 0,string startDate = "",string endDate="",int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<TimeFunctionDto> results = new ListResultObject<TimeFunctionDto>();
            try
            {
                var query = _context.TimeFunctions
                           .Include(x => x.User)
                           .Include(x => x.TimeBreaks)
                           .AsNoTracking();
                
                if (userId > 0)
                {
                    query = query.Where(x => x.UserId == userId);
                }

                if (!string.IsNullOrEmpty(startDate))
                {
                    var sDate = startDate.StringToDate();
                    query = query.Where(x=> x.TimeFunctionStartDate >= sDate);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    var eDate = endDate.StringToDate();
                    query = query.Where(x => x.TimeFunctionEndDate <= eDate);
                }

                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty($"{x.User.FirstName} {x.User.LastName}") && $"{x.User.FirstName} {x.User.LastName}".Contains(searchText))
                        || (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.TimeFunctionStartDate.ToString("yyyy/MMMM/dddd HH:mm")) && x.TimeFunctionStartDate.ToString("yyyy/MMMM/dddd HH:mm").Contains(searchText)) 
                     || (!string.IsNullOrEmpty(x.TimeFunctionEndDate.ToString("yyyy/MMMM/dddd HH:mm")) && x.TimeFunctionEndDate.ToString("yyyy/MMMM/dddd HH:mm").Contains(searchText)) 
                        ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                // کل رکوردهای فیلترشده برای محاسبه مجموع بازه
                var allFilteredItems = await query.ToListAsync();

                var totalRangeBreakTime = TimeSpan.Zero;
                var totalRangeUsefulWorkTime = TimeSpan.Zero;

                foreach (var item in allFilteredItems)
                {
                    var totalWorkTime = item.TimeFunctionEndDate - item.TimeFunctionStartDate;

                    if (totalWorkTime < TimeSpan.Zero)
                        totalWorkTime = TimeSpan.Zero;

                    var totalBreakTime = CalculateTotalBreakTime(item.TimeBreaks);

                    if (totalBreakTime > totalWorkTime)
                        totalBreakTime = totalWorkTime;

                    var totalUsefulWorkTime = totalWorkTime - totalBreakTime;

                    totalRangeBreakTime += totalBreakTime;
                    totalRangeUsefulWorkTime += totalUsefulWorkTime;
                }

                // صفحه فعلی
                var pagedItems = allFilteredItems
                    .OrderByDescending(x => x.CreateDate)
                    .AsQueryable()
                    .SortBy(sortQuery)
                    .ToPaging(pageIndex, pageSize)
                    .ToList();

                results.Results = pagedItems
                    .Select(item =>
                    {
                        var dto = MapTimeFunctionToDto(item);

                        // این دو مقدار برای همه رکوردهای صفحه یکی هستند
                        // چون مربوط به کل بازه تاریخی فیلتر شده‌اند
                        dto.TotalRangeBreakTime = totalRangeBreakTime;
                        dto.TotalRangeUsefulWorkTime = totalRangeUsefulWorkTime;

                        return dto;
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<TimeFunctionDto>> GetTimeFunctionByIdAsync(long TimeFunctionId)
        {
            RowResultObject<TimeFunctionDto> result = new RowResultObject<TimeFunctionDto>();
            try
            {
                var item = await _context.TimeFunctions
            .Include(x => x.User)
            .Include(x => x.TimeBreaks)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == TimeFunctionId);
                result.Result = MapTimeFunctionToDto(item);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTimeFunctionAsync(TimeFunction TimeFunction)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TimeFunctions.Remove(TimeFunction);
                await _context.SaveChangesAsync();
                result.ID = TimeFunction.ID;
                _context.Entry(TimeFunction).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTimeFunctionAsync(long TimeFunctionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var TimeFunctionDto = await GetTimeFunctionByIdAsync(TimeFunctionId);
                var TimeFunction = new TimeFunction()
                {
                    CreateDate = TimeFunctionDto.Result.CreateDate,
                    UpdateDate = TimeFunctionDto.Result.UpdateDate,
                    ID = TimeFunctionDto.Result.ID,
                    OtherLangs = TimeFunctionDto.Result.OtherLangs,
                    IsActive = TimeFunctionDto.Result.IsActive,
                    Description = TimeFunctionDto.Result.Description,
                    UserId = TimeFunctionDto.Result.UserId,
                    TimeFunctionStartDate = TimeFunctionDto.Result.TimeFunctionStartDate,
                    TimeFunctionEndDate = TimeFunctionDto.Result.TimeFunctionEndDate
                };
                result = await RemoveTimeFunctionAsync(TimeFunction);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        private TimeSpan CalculateTotalBreakTime(List<TimeBreak>? timeBreaks)
        {
            if (timeBreaks == null || !timeBreaks.Any())
                return TimeSpan.Zero;

            var totalBreakTime = TimeSpan.Zero;

            foreach (var timeBreak in timeBreaks)
            {
                var breakDuration = timeBreak.TimeBreakEndTime - timeBreak.TimeBreakStartTime;

                // اگر تایم استراحت از نیمه‌شب رد شده باشد
                if (breakDuration < TimeSpan.Zero)
                {
                    breakDuration = breakDuration.Add(TimeSpan.FromDays(1));
                }

                totalBreakTime += breakDuration;
            }

            return totalBreakTime;
        }

        private TimeFunctionDto MapTimeFunctionToDto(TimeFunction item)
        {
            var totalWorkTime = item.TimeFunctionEndDate - item.TimeFunctionStartDate;

            var totalBreakTime = CalculateTotalBreakTime(item.TimeBreaks);

            if (totalBreakTime > totalWorkTime)
            {
                totalBreakTime = totalWorkTime;
            }

            var totalUsefulWorkTime = totalWorkTime - totalBreakTime;

            return new TimeFunctionDto
            {
                ID = item.ID,

                TimeFunctionStartDate = item.TimeFunctionStartDate,
                TimeFunctionEndDate = item.TimeFunctionEndDate,

                UserId = item.UserId,
                UserFullName = item.User != null
                    ? $"{item.User.FirstName} {item.User.LastName}"
                    : null,

                Description = item.Description,

                TimeBreaks = item.TimeBreaks != null
                    ? item.TimeBreaks.Select(x => new TimeBreak
                    {
                        ID = x.ID,
                        TimeBreakStartTime = x.TimeBreakStartTime,
                        TimeBreakEndTime = x.TimeBreakEndTime,
                        Description = x.Description,
                        CreateDate = x.CreateDate,
                        IsActive = x.IsActive,
                        OtherLangs = x.OtherLangs,
                        TimeFunctionId = x.TimeFunctionId,
                        UpdateDate = x.UpdateDate,
                    }).ToList()
                    : new List<TimeBreak>(),

                TotalBreakTime = totalBreakTime,
                TotalUsefulWorkTime = totalUsefulWorkTime,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                IsActive = item.IsActive,
                OtherLangs = item.OtherLangs,
                
            };
        }
    }
}