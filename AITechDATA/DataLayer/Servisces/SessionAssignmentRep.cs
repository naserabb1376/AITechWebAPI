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
    public class SessionAssignmentRep : ISessionAssignmentRep
    {
        private AiITechContext _context;

        public SessionAssignmentRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddSessionAssignmentAsync(SessionAssignment sessionAssignment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.SessionAssignments.AddAsync(sessionAssignment);
                await _context.SaveChangesAsync();
                result.ID = sessionAssignment.ID;
                _context.Entry(sessionAssignment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditSessionAssignmentAsync(SessionAssignment sessionAssignment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.SessionAssignments.Update(sessionAssignment);
                await _context.SaveChangesAsync();
                result.ID = sessionAssignment.ID;
                _context.Entry(sessionAssignment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistSessionAssignmentAsync(long sessionAssignmentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.SessionAssignments
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == sessionAssignmentId);
                result.ID = sessionAssignmentId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<SessionAssignment>> GetAllSessionAssignmentsAsync(long SessionId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<SessionAssignment> results = new ListResultObject<SessionAssignment>();
            try
            {
                var query = _context.SessionAssignments
                    .AsNoTracking()
                    .Where(x =>
                        (SessionId > 0 && x.SessionId == SessionId) ||
                        (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.DueDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Session)
                    .Include(x => x.Assignments)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<SessionAssignment>> GetSessionAssignmentByIdAsync(long sessionAssignmentId)
        {
            RowResultObject<SessionAssignment> result = new RowResultObject<SessionAssignment>();
            try
            {
                result.Result = await _context.SessionAssignments
                    .AsNoTracking()
                    .Include(x => x.Session)
                    .Include(x => x.Assignments)
                    .SingleOrDefaultAsync(x => x.ID == sessionAssignmentId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveSessionAssignmentAsync(SessionAssignment sessionAssignment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.SessionAssignments.Remove(sessionAssignment);
                await _context.SaveChangesAsync();
                result.ID = sessionAssignment.ID;
                _context.Entry(sessionAssignment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveSessionAssignmentAsync(long sessionAssignmentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var sessionAssignment = await GetSessionAssignmentByIdAsync(sessionAssignmentId);
                result = await RemoveSessionAssignmentAsync(sessionAssignment.Result);
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