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
    public class InterviewTimeRep : IInterviewTimeRep
    {
        private AITechContext _context;

        public InterviewTimeRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddInterviewTimeAsync(InterviewTime InterviewTime)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.InterviewTimes.AddAsync(InterviewTime);
                await _context.SaveChangesAsync();
                result.ID = InterviewTime.ID;
                _context.Entry(InterviewTime).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditInterviewTimeAsync(InterviewTime InterviewTime)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.InterviewTimes.Update(InterviewTime);
                await _context.SaveChangesAsync();
                result.ID = InterviewTime.ID;
                _context.Entry(InterviewTime).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistInterviewTimeAsync(long InterviewTimeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.InterviewTimes
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == InterviewTimeId);
                result.ID = InterviewTimeId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<InterviewTime>> GetAllInterviewTimesAsync(long jobRequestId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<InterviewTime> results = new ListResultObject<InterviewTime>();
            try
            {
                var query = _context.InterviewTimes.AsNoTracking();

                if (jobRequestId > 0)
                {
                    query = query.Where(x => x.JobRequestId == jobRequestId);
                }

                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty(x.InterviewDate) && x.InterviewDate.Contains(searchText))
                        || (!string.IsNullOrEmpty(x.InterviewStartTime) && x.InterviewStartTime.Contains(searchText))
                        || (!string.IsNullOrEmpty(x.InterviewEndTime) && x.InterviewEndTime.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.JobRequest.FirstName) && x.JobRequest.FirstName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.LastName) && x.JobRequest.LastName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.FatherName) && x.JobRequest.FatherName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.BirthDate.ToString()) && x.JobRequest.BirthDate.ToString().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.UniversityName) && x.JobRequest.UniversityName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.LastAcademicLicense) && x.JobRequest.LastAcademicLicense.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.EducationalLevel) && x.JobRequest.EducationalLevel.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.EducationStatus) && x.JobRequest.EducationStatus.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.Email) && x.JobRequest.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.PhoneNumber) && x.JobRequest.PhoneNumber.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.RequestedPosition) && x.JobRequest.RequestedPosition.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.CheckStatus) && x.JobRequest.CheckStatus.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.JobRequest.Description) && x.JobRequest.Description.Contains(searchText))
                        ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.JobRequest)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<InterviewTime>> GetInterviewTimeByIdAsync(long InterviewTimeId)
        {
            RowResultObject<InterviewTime> result = new RowResultObject<InterviewTime>();
            try
            {
                result.Result = await _context.InterviewTimes
                    .AsNoTracking()
                    .Include(x => x.JobRequest)
                    .SingleOrDefaultAsync(x => x.ID == InterviewTimeId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveInterviewTimeAsync(InterviewTime InterviewTime)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.InterviewTimes.Remove(InterviewTime);
                await _context.SaveChangesAsync();
                result.ID = InterviewTime.ID;
                _context.Entry(InterviewTime).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveInterviewTimeAsync(long InterviewTimeId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var InterviewTime = await GetInterviewTimeByIdAsync(InterviewTimeId);
                result = await RemoveInterviewTimeAsync(InterviewTime.Result);
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