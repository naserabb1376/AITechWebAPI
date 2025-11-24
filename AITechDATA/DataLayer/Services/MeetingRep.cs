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
    public class MeetingRep : IMeetingRep
    {
        private AITechContext _context;

        public MeetingRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddMeetingAsync(Meeting Meeting)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Meetings.AddAsync(Meeting);
                await _context.SaveChangesAsync();
                result.ID = Meeting.ID;
                _context.Entry(Meeting).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditMeetingAsync(Meeting Meeting)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Meetings.Update(Meeting);
                await _context.SaveChangesAsync();
                result.ID = Meeting.ID;
                _context.Entry(Meeting).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistMeetingAsync(long MeetingId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Meetings
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == MeetingId);
                result.ID = MeetingId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Meeting>> GetAllMeetingsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Meeting> results = new ListResultObject<Meeting>();
            try
            {
                var query = _context.Meetings.AsNoTracking();


                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty(x.MeetingDate.ToString()) && x.MeetingDate.ToString().Contains(searchText))
                        || (!string.IsNullOrEmpty(x.MeetingStartTime.ToString()) && x.MeetingStartTime.ToString().Contains(searchText))
                        || (!string.IsNullOrEmpty(x.MeetingTitle) && x.MeetingTitle.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.MeetingAttendees) && x.MeetingAttendees.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.MeetingStatus) && x.MeetingStatus.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.MeetingOrganizer) && x.MeetingOrganizer.Contains(searchText))
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
        public async Task<RowResultObject<Meeting>> GetMeetingByIdAsync(long MeetingId)
        {
            RowResultObject<Meeting> result = new RowResultObject<Meeting>();
            try
            {
                result.Result = await _context.Meetings
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == MeetingId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveMeetingAsync(Meeting Meeting)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Meetings.Remove(Meeting);
                await _context.SaveChangesAsync();
                result.ID = Meeting.ID;
                _context.Entry(Meeting).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveMeetingAsync(long MeetingId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Meeting = await GetMeetingByIdAsync(MeetingId);
                result = await RemoveMeetingAsync(Meeting.Result);
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