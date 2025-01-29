using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IEventRep
    {
        Task<EventListCustomResponse<Event>> GetAllEventsAsync(long UserId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<EventRowCustomResponse<Event>> GetEventByIdAsync(long eventId);

        Task<BitResultObject> AddEventAsync(Event eventObj);

        Task<BitResultObject> EditEventAsync(Event eventObj);

        Task<BitResultObject> RemoveEventAsync(Event eventObj);

        Task<BitResultObject> RemoveEventAsync(long eventId);

        Task<BitResultObject> ExistEventAsync(long eventId);
    }
}