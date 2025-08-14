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

        public SessionRep(AiITechContext context)
        {
            _context = context;
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

        public async Task<ListResultObject<Session>> GetAllSessionsAsync(
     long groupId = 0,
     long userId = 0,
     int pageIndex = 1,
     int pageSize = 20,
     string searchText = "",
     string sortQuery = "")
        {
            var results = new ListResultObject<Session>();
            try
            {
                IQueryable<Session> query;

                if (userId > 0)
                {
                    query = _context.Attendances
                        .AsNoTracking()
                        .Where(x => x.UserId == userId)
                        .Include(x => x.Session)
                            .ThenInclude(s => s.Group)
                        .Include(x => x.Session)
                            .ThenInclude(s => s.Attendances)
                        .Include(x => x.Session)
                            .ThenInclude(s => s.SessionAssignments)
                        .Select(x => x.Session);
                }
                else
                {
                    query = _context.Sessions
                        .AsNoTracking()
                        .Include(x => x.Group)
                        .Include(x => x.Attendances)
                        .Include(x => x.SessionAssignments);
                }

                if (groupId > 0)
                {
                    query = query.Where(x => x.GroupId == groupId);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(x =>
                        (x.Group != null && x.Group.Name.Contains(searchText)) ||
                        x.SessionDate.ToString().Contains(searchText));
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderByDescending(x => x.SessionDate)
                    .SortBy(sortQuery)
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