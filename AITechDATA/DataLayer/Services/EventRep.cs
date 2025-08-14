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
using AITechDATA.CustomResponses;

namespace AITechDATA.DataLayer.Services
{
    public class EventRep : IEventRep
    {
        private AiITechContext _context;

        public EventRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddEventAsync(Event eventObj)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Events.AddAsync(eventObj);
                await _context.SaveChangesAsync();
                result.ID = eventObj.ID;
                _context.Entry(eventObj).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditEventAsync(Event eventObj)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Events.Update(eventObj);
                await _context.SaveChangesAsync();
                result.ID = eventObj.ID;
                _context.Entry(eventObj).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistEventAsync(long eventId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Events
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == eventId);
                result.ID = eventId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<EventListCustomResponse<Event>> GetAllEventsAsync(long userId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            EventListCustomResponse<Event> results = new EventListCustomResponse<Event>();
            try
            {
                var query = _context.Events
                    .AsNoTracking()
                    .Where(x =>
                        (userId > 0 && x.UserId == userId)
                        || ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Keywords) && x.Keywords.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.EventDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();

                results.ResultImages = results.Results
                    .ToDictionary(
                        user => user,
                        user => _context.Images
                            .Where(img => img.ForeignKeyId == user.ID && img.EntityType == "Event")
                            .ToList()
                    );
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<EventRowCustomResponse<Event>> GetEventByIdAsync(long eventId)
        {
            EventRowCustomResponse<Event> result = new EventRowCustomResponse<Event>();
            try
            {
                result.Result = await _context.Events
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == eventId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<Event, List<Image>?>
{
    { result.Result, await _context.Images
        .Where(img => img.ForeignKeyId == eventId && img.EntityType == "Event")
        .ToListAsync() }
};
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveEventAsync(Event eventObj)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Images = _context.Images.Where(img => img.ForeignKeyId == eventObj.ID && img.EntityType == "Event");
                _context.Images.RemoveRange(Images);

                _context.Events.Remove(eventObj);
                await _context.SaveChangesAsync();
                result.ID = eventObj.ID;
                _context.Entry(eventObj).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveEventAsync(long eventId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var eventObj = await GetEventByIdAsync(eventId);
                result = await RemoveEventAsync(eventObj.Result);
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