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
    public class AssignmentRep : IAssignmentRep
    {
        private AiITechContext _context;

        public AssignmentRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddAssignmentAsync(Assignment assignment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Assignments.AddAsync(assignment);
                await _context.SaveChangesAsync();
                result.ID = assignment.ID;
                _context.Entry(assignment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditAssignmentAsync(Assignment assignment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Assignments.Update(assignment);
                await _context.SaveChangesAsync();
                result.ID = assignment.ID;
                _context.Entry(assignment).State = EntityState.Detached;
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

        public async Task<ListResultObject<Assignment>> GetAllAssignmentsAsync(long sessionAssignmentId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Assignment> results = new ListResultObject<Assignment>();
            try
            {
                var query = _context.Assignments
                    .AsNoTracking()
                    .Where(x =>
                         (sessionAssignmentId > 0 && x.SessionAssignmentId == sessionAssignmentId) 
                       || ((!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchText)) ||
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
                    .Include(x => x.Files)
                    .SingleOrDefaultAsync(x => x.ID == assignmentId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAssignmentAsync(Assignment assignment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
                result.ID = assignment.ID;
                _context.Entry(assignment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveAssignmentAsync(long assignmentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var assignment = await GetAssignmentByIdAsync(assignmentId);
                result = await RemoveAssignmentAsync(assignment.Result);
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