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
    public class AssignmentRep : IAssignmentRep
    {
        private AITechContext _context;

        public AssignmentRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddAssignmentsAsync(List<Assignment> assignments)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Assignments.AddRangeAsync(assignments);
                await _context.SaveChangesAsync();
                result.ID = assignments.FirstOrDefault().ID;
                foreach (var assignment in assignments)
                {
                    _context.Entry(assignment).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditAssignmentsAsync(List<Assignment> assignments)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Assignments.UpdateRange(assignments);
                await _context.SaveChangesAsync();
                result.ID = assignments.FirstOrDefault().ID;
                foreach (var assignment in assignments)
                {
                    _context.Entry(assignment).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistAssignmentAsync(long assignmentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Assignments
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == assignmentId);
                result.ID = assignmentId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Assignment>> GetAllAssignmentsAsync(long UserId = 0, long sessionAssignmentId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Assignment> results = new ListResultObject<Assignment>();
            try
            {
                var query = _context.Assignments.AsNoTracking();
                if (UserId > 0)
                {
                    query = query.Where(x => x.UserId == UserId);
                }
                if (sessionAssignmentId > 0)
                {
                    query = query.Where(x => x.SessionAssignmentId == sessionAssignmentId);
                }
                query = query.Where(x =>
                         ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.SubmissionDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Assignment>> GetAssignmentByIdAsync(long assignmentId)
        {
            RowResultObject<Assignment> result = new RowResultObject<Assignment>();
            try
            {
                result.Result = await _context.Assignments
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.SessionAssignment)
                    //.Include(x => x.Files)
                    .SingleOrDefaultAsync(x => x.ID == assignmentId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAssignmentsAsync(List<Assignment> assignments)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Assignments.RemoveRange(assignments);
                await _context.SaveChangesAsync();
                result.ID = assignments.FirstOrDefault().ID;
                foreach (var assignment in assignments)
                {
                    _context.Entry(assignment).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAssignmentsAsync(List<long> assignmentIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var assignmentsToRemove = new List<Assignment>();

                foreach (var assignmentId in assignmentIds)
                {
                    var assignment = await GetAssignmentByIdAsync(assignmentId);
                    if (assignment.Result != null)
                    {
                        assignmentsToRemove.Add(assignment.Result);
                    }
                }

                if (assignmentsToRemove.Any())
                {
                    result = await RemoveAssignmentsAsync(assignmentsToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching assignments found to remove.";
                }
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