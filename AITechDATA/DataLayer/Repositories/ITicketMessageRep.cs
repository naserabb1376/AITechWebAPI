using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ITicketMessageRep
    {
        Task<ListResultObject<TicketMessage>> GetAllTicketMessagesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<TicketMessage>> GetTicketMessageByIdAsync(long ticketMessageId);

        Task<BitResultObject> AddTicketMessageAsync(TicketMessage ticketMessage);

        Task<BitResultObject> EditTicketMessageAsync(TicketMessage ticketMessage);

        Task<BitResultObject> RemoveTicketMessageAsync(TicketMessage ticketMessage);

        Task<BitResultObject> RemoveTicketMessageAsync(long ticketMessageId);

        Task<BitResultObject> ExistTicketMessageAsync(long ticketMessageId);
    }
}