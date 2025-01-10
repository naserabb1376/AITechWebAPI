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
    public class AttendanceRep : IAttendanceRep
    {
        private AiITechContext _context;

        public AttendanceRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddAttendanceAsync(Attendance attendance)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Attendances.AddAsync(attendance);
                await _context.SaveChangesAsync();
                result.ID = attendance.ID;
                _context.Entry(attendance).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditAttendanceAsync(Attendance attendance)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Attendances.Update(attendance);
                await _context.SaveChangesAsync();
                result.ID = attendance.ID;
                _context.Entry(attendance).State = EntityState.Detached;
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

        public async Task<ListResultObject<Attendance>> GetAllAttendancesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Attendance> results = new ListResultObject<Attendance>();
            try
            {
                var query = _context.Attendances
                    .AsNoTracking()
                    .Where(x =>
                        x.User.ID.ToString().Contains(searchText) ||
                        x.Session.ID.ToString().Contains(searchText)
                    );

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

        public async Task<BitResultObject> RemoveAttendanceAsync(Attendance attendance)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
                result.ID = attendance.ID;
                _context.Entry(attendance).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAttendanceAsync(long attendanceId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var attendance = await GetAttendanceByIdAsync(attendanceId);
                result = await RemoveAttendanceAsync(attendance.Result);
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