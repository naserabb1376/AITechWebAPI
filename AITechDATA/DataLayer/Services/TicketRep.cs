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
    public class TicketRep : ITicketRep
    {
        private AITechContext _context;

        public TicketRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddTicketAsync(Ticket ticket)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                if (ticket.TeacherId > 0)
                {
                    var existUserGroup = await _context.UserGroups.Include(x => x.Group).AnyAsync(x=> x.UserId == ticket.UserId && x.Group.TeacherId == ticket.TeacherId);

                    if (!existUserGroup)
                    {
                        result.Status = false;
                        result.ErrorMessage = "شما با این مدرس درسی ندارید";
                        return result;
                    }
                }
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

                if (ticket.TeacherId > 0)
                {
                    var existUserGroup = await _context.UserGroups.Include(x => x.Group).AnyAsync(x => x.UserId == ticket.UserId && x.Group.TeacherId == ticket.TeacherId);

                    if (!existUserGroup)
                    {
                        result.Status = false;
                        result.ErrorMessage = "شما با این مدرس درسی ندارید";
                        return result;
                    }
                }

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

        public async Task<TicketListCustomResponse<Ticket>> GetAllTicketsAsync(long UserId = 0, long TeacherId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            TicketListCustomResponse<Ticket> results = new TicketListCustomResponse<Ticket>();
            try
            {
                var query = _context.Tickets.Include(x => x.Messages).Include(x=> x.User).Include(x=> x.Teacher).AsNoTracking();

                if (UserId > 0)
                {
                    query = query.Where(x => x.UserId == UserId);
                }

                if (TeacherId > 0)
                {
                    query = query.Where(x => x.TeacherId == TeacherId);
                }

                query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.Subject) && x.Subject.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.User.Username) && x.Subject.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .Include(x => x.Messages)
                    .ToListAsync();


                results.ResultImages = results.Results
    .ToDictionary(
        user => user,
        user => _context.Images
            .Where(img => img.ForeignKeyId == user.ID && img.EntityType == "Ticket")
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

        public async Task<TicketRowCustomResponse<Ticket>> GetTicketByIdAsync(long ticketId)
        {
            TicketRowCustomResponse<Ticket> result = new TicketRowCustomResponse<Ticket>();
            try
            {
                result.Result = await _context.Tickets
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Teacher)
                    .Include(x => x.Messages)
                    .SingleOrDefaultAsync(x => x.ID == ticketId);

                if (result.Result != null)
                {
                    result.ResultImages = new Dictionary<Ticket, List<Image>?>
{
    { result.Result, await _context.Images
        .Where(img => img.ForeignKeyId == ticketId && img.EntityType == "Ticket")
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

        public async Task<BitResultObject> RemoveTicketAsync(Ticket ticket)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Images = _context.Images.Where(img => img.ForeignKeyId == ticket.ID && img.EntityType == "Ticket");
                _context.Images.RemoveRange(Images);

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