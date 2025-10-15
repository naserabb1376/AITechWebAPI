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
    public class InterviewTimeRep : IInterviewTimeRep
    {
        private AITechContext _context;

        public InterviewTimeRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddInterviewTimeAsync(InterviewTime InterviewTime)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.InterviewTimes.AddAsync(InterviewTime);
                await _context.SaveChangesAsync();
                result.ID = InterviewTime.ID;
                _context.Entry(InterviewTime).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditInterviewTimeAsync(InterviewTime InterviewTime)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.InterviewTimes.Update(InterviewTime);
                await _context.SaveChangesAsync();
                result.ID = InterviewTime.ID;
                _context.Entry(InterviewTime).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistInterviewTimeAsync(long InterviewTimeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.InterviewTimes
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == InterviewTimeId);
                result.ID = InterviewTimeId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<InterviewTime>> GetAllInterviewTimesAsync(long jobRequestId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<InterviewTime> results = new ListResultObject<InterviewTime>();
            try
            {
                var query = _context.InterviewTimes.AsNoTracking();

                if (jobRequestId > 0)
                {
                    query = query.Where(x => x.JobRequestId == jobRequestId);
                }

                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty(x.InterviewDate) && x.InterviewDate.Contains(searchText))
                        || (!string.IsNullOrEmpty(x.InterviewStartTime) && x.InterviewStartTime.Contains(searchText))
                        || (!string.IsNullOrEmpty(x.InterviewEndTime) && x.InterviewEndTime.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.JobRequest.FirstName) && x.JobRequest.FirstName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.LastName) && x.JobRequest.LastName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.FatherName) && x.JobRequest.FatherName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.BirthDate.ToString()) && x.JobRequest.BirthDate.ToString().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.UniversityName) && x.JobRequest.UniversityName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.LastAcademicLicense) && x.JobRequest.LastAcademicLicense.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.EducationalLevel) && x.JobRequest.EducationalLevel.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.EducationStatus) && x.JobRequest.EducationStatus.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.Email) && x.JobRequest.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.PhoneNumber) && x.JobRequest.PhoneNumber.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.RequestedPosition) && x.JobRequest.RequestedPosition.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.CheckStatus) && x.JobRequest.CheckStatus.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.Description) && x.JobRequest.Description.Contains(searchText))
                        ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.JobRequest)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<ListResultObject<InterviewSlot>> GetInterviewTimesInDayAsync(string interviewDate, string interviewStartTime, string interViewEndTime, int slotMinutes, int pageIndex = 1, int pageSize = 20)
        {
            ListResultObject<InterviewSlot> results = new ListResultObject<InterviewSlot>();
            try
            {
                // 🗓 تبدیل تاریخ شمسی به میلادی
                PersianCalendar pc = new PersianCalendar();
                var dateParts = interviewDate.Split('/');
                int year = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int day = int.Parse(dateParts[2]);

                DateTime baseDate = pc.ToDateTime(year, month, day, 0, 0, 0, 0);

                // ⏰ تبدیل ساعت شروع و پایان به DateTime
                var startParts = interviewStartTime.Split(':');
                var endParts = interViewEndTime.Split(':');
                DateTime startDateTime = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day,
                    int.Parse(startParts[0]), int.Parse(startParts[1]), 0);
                DateTime endDateTime = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day,
                    int.Parse(endParts[0]), int.Parse(endParts[1]), 0);

                // 🟡 دریافت وقت‌های رزرو شده از دیتابیس
                var reservedTimes = await _context.InterviewTimes
                    .Where(x => x.InterviewDate == interviewDate)
                    .Select(x => x.InterviewStartTime)
                    .ToListAsync();

                // ⏳ ایجاد بازه‌های مصاحبه
                DateTime current = startDateTime;
                while (current.AddMinutes(slotMinutes) <= endDateTime)
                {
                    var slotStart = current.ToString("HH:mm");
                    var slotEnd = current.AddMinutes(slotMinutes).ToString("HH:mm");
                    var slot = new InterviewSlot
                    {
                        InterviewDate = interviewDate,
                        InterviewStartTime = slotStart,
                        InterviewEndTime = slotEnd,
                        IsReserved = reservedTimes.Contains(slotStart)  // 🟢 بررسی رزرو بودن
                    };
                    results.Results.Add(slot);
                    current = current.AddMinutes(slotMinutes);
                }
                var query = results.Results.AsQueryable();  
                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = query.ToPaging(pageIndex, pageSize).ToList();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }
        public async Task<RowResultObject<InterviewTime>> GetInterviewTimeByIdAsync(long InterviewTimeId)
        {
            RowResultObject<InterviewTime> result = new RowResultObject<InterviewTime>();
            try
            {
                result.Result = await _context.InterviewTimes
                    .AsNoTracking()
                    .Include(x => x.JobRequest)
                    .SingleOrDefaultAsync(x => x.ID == InterviewTimeId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveInterviewTimeAsync(InterviewTime InterviewTime)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.InterviewTimes.Remove(InterviewTime);
                await _context.SaveChangesAsync();
                result.ID = InterviewTime.ID;
                _context.Entry(InterviewTime).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveInterviewTimeAsync(long InterviewTimeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var InterviewTime = await GetInterviewTimeByIdAsync(InterviewTimeId);
                result = await RemoveInterviewTimeAsync(InterviewTime.Result);
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