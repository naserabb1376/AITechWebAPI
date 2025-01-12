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
    public class SessionRep : ISessionRep
    {
        private AiITechContext _context;

        public SessionRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddSessionAsync(Session session)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Sessions.AddAsync(session);
                await _context.SaveChangesAsync();
                result.ID = session.ID;
                _context.Entry(session).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditSessionAsync(Session session)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Sessions.Update(session);
                await _context.SaveChangesAsync();
                result.ID = session.ID;
                _context.Entry(session).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistSessionAsync(long sessionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Sessions
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == sessionId);
                result.ID = sessionId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Session>> GetAllSessionsAsync(long GroupId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Session> results = new ListResultObject<Session>();
            try
            {
                var query = _context.Sessions
                    .AsNoTracking()
                    .Where(x =>
                        (GroupId > 0 && x.GroupId == GroupId) ||
                        x.Group.Name.Contains(searchText) ||
                        x.SessionDate.ToString().Contains(searchText)
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.SessionDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Group)
                    .Include(x => x.Attendances)
                    .Include(x => x.SessionAssignments)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Session>> GetSessionByIdAsync(long sessionId)
        {
            RowResultObject<Session> result = new RowResultObject<Session>();
            try
            {
                result.Result = await _context.Sessions
                    .AsNoTracking()
                    .Include(x => x.Group)
                    .Include(x => x.Attendances)
                    .Include(x => x.SessionAssignments)
                    .SingleOrDefaultAsync(x => x.ID == sessionId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveSessionAsync(Session session)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
                result.ID = session.ID;
                _context.Entry(session).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveSessionAsync(long sessionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var session = await GetSessionByIdAsync(sessionId);
                result = await RemoveSessionAsync(session.Result);
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