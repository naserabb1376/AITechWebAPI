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
    public class AttendanceRep : IAttendanceRep
    {
        private AITechContext _context;

        public AttendanceRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddAttendancesAsync(List<Attendance> attendances)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Attendances.AddRangeAsync(attendances);
                await _context.SaveChangesAsync();
                result.ID = attendances.FirstOrDefault().ID;
                foreach (var attendance in attendances)
                {
                    _context.Entry(attendance).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditAttendancesAsync(List<Attendance> attendances)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Attendances.UpdateRange(attendances);
                await _context.SaveChangesAsync();
                result.ID = attendances.FirstOrDefault().ID;
                foreach (var attendance in attendances)
                {
                    _context.Entry(attendance).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistAttendanceAsync(long attendanceId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Attendances
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == attendanceId);
                result.ID = attendanceId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Attendance>> GetAllAttendancesAsync(long userId = 0, long sessionId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Attendance> results = new ListResultObject<Attendance>();
            try
            {
                var query = _context.Attendances.AsNoTracking();
                if (userId > 0)
                {
                    query = query.Where(x => x.UserId == userId);
                }
                if (sessionId > 0)
                {
                    query = query.Where(x => x.SessionId == sessionId);
                }
                query =query.Where(x =>
                        ((x.User.FullName.ToString().Contains(searchText) ||
                        x.Session.SessionDate.ToString().Contains(searchText))
                    ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ID)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .Include(x => x.Session)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Attendance>> GetAttendanceByIdAsync(long attendanceId)
        {
            RowResultObject<Attendance> result = new RowResultObject<Attendance>();
            try
            {
                result.Result = await _context.Attendances
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Session)
                    .SingleOrDefaultAsync(x => x.ID == attendanceId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAttendancesAsync(List<Attendance> attendances)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Attendances.RemoveRange(attendances);
                await _context.SaveChangesAsync();
                result.ID = attendances.FirstOrDefault().ID;
                foreach (var attendance in attendances)
                {
                    _context.Entry(attendance).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAttendancesAsync(List<long> attendanceIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var AttendancesToRemove = new List<Attendance>();

                foreach (var AttendanceId in attendanceIds)
                {
                    var Attendance = await GetAttendanceByIdAsync(AttendanceId);
                    if (Attendance.Result != null)
                    {
                        AttendancesToRemove.Add(Attendance.Result);
                    }
                }

                if (AttendancesToRemove.Any())
                {
                    result = await RemoveAttendancesAsync(AttendancesToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching Attendances found to remove.";
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