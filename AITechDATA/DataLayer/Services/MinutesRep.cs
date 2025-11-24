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
    public class MinutesRep : IMinutesRep
    {
        private AITechContext _context;

        public MinutesRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddMinutesAsync(Minutes Minutes)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Minutes.AddAsync(Minutes);
                await _context.SaveChangesAsync();
                result.ID = Minutes.ID;
                _context.Entry(Minutes).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditMinutesAsync(Minutes Minutes)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Minutes.Update(Minutes);
                await _context.SaveChangesAsync();
                result.ID = Minutes.ID;
                _context.Entry(Minutes).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistMinutesAsync(long MinutesId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Minutes
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == MinutesId);
                result.ID = MinutesId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Minutes>> GetAllMinutesAsync(long MeetingId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Minutes> results = new ListResultObject<Minutes>();
            try
            {
                var query = _context.Minutes.AsNoTracking();

                if (MeetingId > 0)
                {
                    query = query.Where(x => x.MeetingId == MeetingId);
                }

                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty(x.MinutesSubject) && x.MinutesSubject.Contains(searchText))
                        || (!string.IsNullOrEmpty(x.MinutesDescription) && x.MinutesDescription.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.Meeting.MeetingTitle) && x.Meeting.MeetingTitle.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Meeting.MeetingOrganizer) && x.Meeting.MeetingOrganizer.Contains(searchText))
                        ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Meeting)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Minutes>> GetMinutesByIdAsync(long MinutesId)
        {
            RowResultObject<Minutes> result = new RowResultObject<Minutes>();
            try
            {
                result.Result = await _context.Minutes
                    .AsNoTracking()
                    .Include(x => x.Meeting)
                    .SingleOrDefaultAsync(x => x.ID == MinutesId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveMinutesAsync(Minutes Minutes)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Minutes.Remove(Minutes);
                await _context.SaveChangesAsync();
                result.ID = Minutes.ID;
                _context.Entry(Minutes).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveMinutesAsync(long MinutesId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Minutes = await GetMinutesByIdAsync(MinutesId);
                result = await RemoveMinutesAsync(Minutes.Result);
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