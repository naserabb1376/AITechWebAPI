using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ITicketRep
    {
        Task<ListResultObject<Ticket>> GetAllTicketsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<Ticket>> GetTicketByIdAsync(long ticketId);

        Task<BitResultObject> AddTicketAsync(Ticket ticket);

        Task<BitResultObject> EditTicketAsync(Ticket ticket);

        Task<BitResultObject> RemoveTicketAsync(Ticket ticket);

        Task<BitResultObject> RemoveTicketAsync(long ticketId);

        Task<BitResultObject> ExistTicketAsync(long ticketId);
    }
}