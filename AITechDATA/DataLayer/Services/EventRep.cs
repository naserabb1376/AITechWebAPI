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
        private AITechContext _context;

        public EventRep(AITechContext context)
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

        public async Task<EventListCustomResponse<EventDto>> GetAllEventsAsync(long ClientUserId, long ClientRoleId, long userId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            EventListCustomResponse<EventDto> results = new EventListCustomResponse<EventDto>();
            try
            {
                var query = _context.Events.Include(x => x.User).AsNoTracking();
                if (userId > 0)
                {
                    query = query.Where(x => x.UserId == userId);
                }
                query = query.Where(x =>
                        ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Note) && x.Note.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Keywords) && x.Keywords.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.Select(x=> new EventDto()
                {
                    CreateDate = x.CreateDate,
                    UpdateDate = x.UpdateDate,
                    User = x.User,
                    UserId = x.UserId,
                    ID = x.ID,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    OtherLangs = x.OtherLangs,
                    Note = x.Note,
                    EventDate = x.EventDate,
                    Keywords = x.Keywords,
                    Fee = x.Fee,
                    Title = x.Title,
                    DiscountPercent = _context.GetDiscount(x.Fee ?? 0, "event", x.ID, ClientUserId, ClientRoleId).DiscountPercent,
                    DiscountedFee = _context.GetDiscount(x.Fee ?? 0, "event", x.ID, ClientUserId, ClientRoleId).DiscountedFee

                })
                    
                    .OrderByDescending(x => x.EventDate)
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

        public async Task<EventRowCustomResponse<EventDto>> GetEventByIdAsync(long eventId, long ClientUserId=0, long ClientRoleId=0)
        {
            EventRowCustomResponse<EventDto> result = new EventRowCustomResponse<EventDto>();
            try
            {
                result.Result = await _context.Events
                    .AsNoTracking()
                    .Select(x => new EventDto()
                    {
                        CreateDate = x.CreateDate,
                        UpdateDate = x.UpdateDate,
                        User = x.User,
                        UserId = x.UserId,
                        ID = x.ID,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        OtherLangs = x.OtherLangs,
                        Note = x.Note,
                        EventDate = x.EventDate,
                        Keywords = x.Keywords,
                        Fee = x.Fee,
                        Title = x.Title,
                        DiscountPercent = _context.GetDiscount(x.Fee ?? 0, "event", x.ID, ClientUserId, ClientRoleId).DiscountPercent,
                        DiscountedFee = _context.GetDiscount(x.Fee ?? 0, "event", x.ID, ClientUserId, ClientRoleId).DiscountedFee

                    })
                    .SingleOrDefaultAsync(x => x.ID == eventId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<EventDto, List<Image>?>
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
                var x = eventObj.Result;
                var theEvent = new Event()
                {
                    CreateDate = x.CreateDate,
                    UpdateDate = x.UpdateDate,
                    User = x.User,
                    UserId = x.UserId,
                    ID = x.ID,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    OtherLangs = x.OtherLangs,
                    Note = x.Note,
                    EventDate = x.EventDate,
                    Keywords = x.Keywords,
                    Fee = x.Fee,
                    Title = x.Title,

                };
                result = await RemoveEventAsync(theEvent);
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