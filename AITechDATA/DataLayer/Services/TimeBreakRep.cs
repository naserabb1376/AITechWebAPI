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
    public class TimeBreakRep : ITimeBreakRep
    {
        private AITechContext _context;

        public TimeBreakRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddTimeBreakAsync(TimeBreak TimeBreak)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.TimeBreaks.AddAsync(TimeBreak);
                await _context.SaveChangesAsync();
                result.ID = TimeBreak.ID;
                _context.Entry(TimeBreak).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditTimeBreakAsync(TimeBreak TimeBreak)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TimeBreaks.Update(TimeBreak);
                await _context.SaveChangesAsync();
                result.ID = TimeBreak.ID;
                _context.Entry(TimeBreak).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistTimeBreakAsync(long TimeBreakId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.TimeBreaks
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == TimeBreakId);
                result.ID = TimeBreakId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<TimeBreak>> GetAllTimeBreaksAsync(long timeFunctionId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<TimeBreak> results = new ListResultObject<TimeBreak>();
            try
            {
                var query = _context.TimeBreaks.Include(x=> x.TimeFunction).AsNoTracking();

                if (timeFunctionId > 0)
                {
                    query = query.Where(x => x.TimeFunctionId == timeFunctionId);
                }

        

                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.TimeBreakStartTime.ToString("HH:mm")) && x.TimeBreakStartTime.ToString("HH:mm").Contains(searchText))
                     || (!string.IsNullOrEmpty(x.TimeBreakEndTime.ToString("HH:mm")) && x.TimeBreakEndTime.ToString("HH:mm").Contains(searchText))
                     || (!string.IsNullOrEmpty(x.TimeFunction.Description) && x.TimeFunction.Description.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.TimeFunction.TimeFunctionStartDate.ToString("yyyy/MMMM/dddd HH:mm")) && x.TimeFunction.TimeFunctionStartDate.ToString("yyyy/MMMM/dddd HH:mm").Contains(searchText)) 
                     || (!string.IsNullOrEmpty(x.TimeFunction.TimeFunctionEndDate.ToString("yyyy/MMMM/dddd HH:mm")) && x.TimeFunction.TimeFunctionEndDate.ToString("yyyy/MMMM/dddd HH:mm").Contains(searchText)) 
                        ));

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

        public async Task<RowResultObject<TimeBreak>> GetTimeBreakByIdAsync(long TimeBreakId)
        {
            RowResultObject<TimeBreak> result = new RowResultObject<TimeBreak>();
            try
            {
                result.Result = await _context.TimeBreaks.Include(x=> x.TimeFunction)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == TimeBreakId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTimeBreakAsync(TimeBreak TimeBreak)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TimeBreaks.Remove(TimeBreak);
                await _context.SaveChangesAsync();
                result.ID = TimeBreak.ID;
                _context.Entry(TimeBreak).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTimeBreakAsync(long TimeBreakId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var TimeBreak = await GetTimeBreakByIdAsync(TimeBreakId);
                result = await RemoveTimeBreakAsync(TimeBreak.Result);
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