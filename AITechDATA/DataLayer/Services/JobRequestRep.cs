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
    public class JobRequestRep : IJobRequestRep
    {
        private AiITechContext _context;

        public JobRequestRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddJobRequestAsync(JobRequest JobRequest)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.JobRequests.AddAsync(JobRequest);
                await _context.SaveChangesAsync();
                result.ID = JobRequest.ID;
                _context.Entry(JobRequest).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditJobRequestAsync(JobRequest JobRequest)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.JobRequests.Update(JobRequest);
                await _context.SaveChangesAsync();
                result.ID = JobRequest.ID;
                _context.Entry(JobRequest).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistJobRequestAsync(long JobRequestId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.JobRequests
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == JobRequestId);
                result.ID = JobRequestId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<JobRequest>> GetAllJobRequestsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<JobRequest> results = new ListResultObject<JobRequest>();
            try
            {
                var query = _context.JobRequests
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.RequestedPosition) && x.RequestedPosition.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
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

        public async Task<RowResultObject<JobRequest>> GetJobRequestByIdAsync(long JobRequestId)
        {
            RowResultObject<JobRequest> result = new RowResultObject<JobRequest>();
            try
            {
                result.Result = await _context.JobRequests
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == JobRequestId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveJobRequestAsync(JobRequest JobRequest)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.JobRequests.Remove(JobRequest);
                await _context.SaveChangesAsync();
                result.ID = JobRequest.ID;
                _context.Entry(JobRequest).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveJobRequestAsync(long JobRequestId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var JobRequest = await GetJobRequestByIdAsync(JobRequestId);
                result = await RemoveJobRequestAsync(JobRequest.Result);
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