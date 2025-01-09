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
    public class EventRep : IEventRep
    {
        private AiITechContext _context;

        public EventRep()
        {
            _context = DbTools.GetDbContext();
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

        public async Task<ListResultObject<Event>> GetAllEventsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "")
        {
            ListResultObject<Event> results = new ListResultObject<Event>();
            try
            {
                var query = _context.Events
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Keywords) && x.Keywords.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.EventDate)
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

        public async Task<RowResultObject<Event>> GetEventByIdAsync(long eventId)
        {
            RowResultObject<Event> result = new RowResultObject<Event>();
            try
            {
                result.Result = await _context.Events
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == eventId);
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