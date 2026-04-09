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
    public class DutyRep : IDutyRep
    {
        private AITechContext _context;

        public DutyRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddDutyAsync(Duty Duty)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Duties.AddAsync(Duty);
                await _context.SaveChangesAsync();
                result.ID = Duty.ID;
                _context.Entry(Duty).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditDutyAsync(Duty Duty)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Duties.Update(Duty);
                await _context.SaveChangesAsync();
                result.ID = Duty.ID;
                _context.Entry(Duty).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistDutyAsync(long DutyId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Duties
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == DutyId);
                result.ID = DutyId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Duty>> GetAllDutiesAsync(long userId = 0, long senderUserId = 0, int readState = 2, int doneState = 2, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<Duty> results = new ListResultObject<Duty>();
            try
            {
                var query = _context.Duties.Include(x=> x.User).Include(x=> x.SenderUser).AsNoTracking();

                if (userId > 0)
                {
                    query = query.Where(x => x.UserId == userId);
                }

                if (senderUserId > 0)
                {
                    query = query.Where(x => x.SenderUserId == senderUserId);
                }

                if (readState < 2)
                {
                    query = query.Where(x => x.IsRead == Convert.ToBoolean(readState));
                }

                if (doneState < 2)
                {
                    query = query.Where(x => x.IsDone == Convert.ToBoolean(doneState));
                }

                query = query.Where(x =>
                        ((!string.IsNullOrEmpty(x.DutyTitle) && x.DutyTitle.Contains(searchText)) ||
                        ((!string.IsNullOrEmpty(x.DutyDescription) && x.DutyDescription.Contains(searchText)) ||
                        ((!string.IsNullOrEmpty(x.DutyReport) && x.DutyReport.Contains(searchText))
                        ))));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Duty>> GetDutyByIdAsync(long DutyId)
        {
            RowResultObject<Duty> result = new RowResultObject<Duty>();
            try
            {
                result.Result = await _context.Duties
                    .AsNoTracking()
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.ID == DutyId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDutyAsync(Duty Duty)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Duties.Remove(Duty);
                await _context.SaveChangesAsync();
                result.ID = Duty.ID;
                _context.Entry(Duty).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDutyAsync(long DutyId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Duty = await GetDutyByIdAsync(DutyId);
                result = await RemoveDutyAsync(Duty.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        // Chart

        public async Task<ListResultObject<EmployeePerformanceChartDto>> GetEmployeePerformanceChartAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    long userId = 0)
        {
            ListResultObject<EmployeePerformanceChartDto> results = new ListResultObject<EmployeePerformanceChartDto>();
            try
            {
                var query = _context.Duties
              .Include(x => x.User)
              .AsNoTracking()
              .AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(x => x.CreateDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(x => x.CreateDate <= toDate.Value);

                if (userId > 0)
                    query = query.Where(x => x.UserId == userId);

                var result = await query
                    .GroupBy(x => new { x.UserId, FullName = $"{x.User.FirstName} {x.User.LastName}" })
                    .Select(g => new EmployeePerformanceChartDto
                    {
                        UserId = g.Key.UserId,
                        FullName = g.Key.FullName,
                        TotalDuties = g.Count(),
                        DoneCount = g.Count(x => x.IsDone),
                        NotDoneCount = g.Count(x => !x.IsDone),
                        ReadCount = g.Count(x => x.IsRead),
                        NotReadCount = g.Count(x => !x.IsRead),
                        AverageScore = g.Any() ? g.Average(x => x.DutyScore) : 0,
                        DonePercentage = g.Any()
                            ? (float)g.Count(x => x.IsDone) * 100 / g.Count()
                            : 0
                    })
                    .OrderByDescending(x => x.AverageScore)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
          
        }

        public async Task<ListResultObject<DutyTrendChartDto>> GetDutyTrendChartAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    long userId = 0)
        {
            ListResultObject<DutyTrendChartDto> results = new ListResultObject<DutyTrendChartDto>();
            try
            {

                var query = _context.Duties
                .AsNoTracking()
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.CreateDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.CreateDate <= toDate.Value);

            if (userId > 0)
                query = query.Where(x => x.UserId == userId);

            var result = await query
                .GroupBy(x => x.CreateDate)
                .Select(g => new DutyTrendChartDto
                {
                    Date = g.Key.Value.ToShamsi().ToString("yyyy-MM-dd"),
                    TotalCount = g.Count(),
                    DoneCount = g.Count(x => x.IsDone)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }
    }
}