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
    public class TicketRep : ITicketRep
    {
        private AiITechContext _context;

        public TicketRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddTicketAsync(Ticket ticket)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();
                result.ID = ticket.ID;
                _context.Entry(ticket).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditTicketAsync(Ticket ticket)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
                result.ID = ticket.ID;
                _context.Entry(ticket).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistTicketAsync(long ticketId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Tickets
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == ticketId);
                result.ID = ticketId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Ticket>> GetAllTicketsAsync(long UserId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Ticket> results = new ListResultObject<Ticket>();
            try
            {
                var query = _context.Tickets
                    .AsNoTracking()
                    .Where(x =>
                        (UserId > 0 && x.UserId == UserId) ||

                        (!string.IsNullOrEmpty(x.Subject) && x.Subject.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .Include(x => x.Messages)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Ticket>> GetTicketByIdAsync(long ticketId)
        {
            RowResultObject<Ticket> result = new RowResultObject<Ticket>();
            try
            {
                result.Result = await _context.Tickets
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Messages)
                    .SingleOrDefaultAsync(x => x.ID == ticketId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTicketAsync(Ticket ticket)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                result.ID = ticket.ID;
                _context.Entry(ticket).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTicketAsync(long ticketId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var ticket = await GetTicketByIdAsync(ticketId);
                result = await RemoveTicketAsync(ticket.Result);
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