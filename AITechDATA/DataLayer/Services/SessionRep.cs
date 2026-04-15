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
        private AITechContext _context;

        public SessionRep(AITechContext context)
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
                // Base Query (از Session شروع می‌کنیم)
                IQueryable<Session> query = _context.Sessions
                    .AsNoTracking()
                    .Include(s => s.Group).ThenInclude(g => g.Teacher)
                    .Include(s => s.Group).ThenInclude(g => g.Course)
                    .Include(s => s.Attendances)
                    .Include(s => s.SessionAssignments);

                // Filter: فقط Session های کاربر
                if (userId > 0)
                {
                    query = query.Where(s => s.Attendances.Any(a => a.UserId == userId));
                }

                // Filter: group
                if (groupId > 0)
                {
                    query = query.Where(s => s.GroupId == groupId);
                }

                // Search Text
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(s =>
                        (s.Group != null && s.Group.Name.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(s.Description) && s.Description.Contains(searchText)) ||
                        s.SessionDate.ToString().Contains(searchText)
                    );
                }

                // Count
                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                // Sorting + Paging + Final Result
                results.Results = await query
                    .OrderByDescending(s => s.SessionDate)
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
                        .Include(x => x.Group).ThenInclude(x => x.Teacher)
                        .Include(x => x.Group).ThenInclude(x => x.Course)
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