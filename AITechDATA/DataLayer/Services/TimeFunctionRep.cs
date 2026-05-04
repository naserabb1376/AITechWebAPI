using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace AITechDATA.DataLayer.Services
{
    public class TimeFunctionRep : ITimeFunctionRep
    {
        private AITechContext _context;

        public TimeFunctionRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddTimeFunctionAsync(TimeFunction TimeFunction)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.TimeFunctions.AddAsync(TimeFunction);
                await _context.SaveChangesAsync();
                result.ID = TimeFunction.ID;
                _context.Entry(TimeFunction).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditTimeFunctionAsync(TimeFunction TimeFunction)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TimeFunctions.Update(TimeFunction);
                await _context.SaveChangesAsync();
                result.ID = TimeFunction.ID;
                _context.Entry(TimeFunction).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistTimeFunctionAsync(long TimeFunctionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.TimeFunctions
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == TimeFunctionId);
                result.ID = TimeFunctionId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<TimeFunction>> GetAllTimeFunctionsAsync(long userId = 0,string startDate = "",string endDate="",int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<TimeFunction> results = new ListResultObject<TimeFunction>();
            try
            {
                var query = _context.TimeFunctions.Include(x=> x.User).AsNoTracking();

                if (userId > 0)
                {
                    query = query.Where(x => x.UserId == userId);
                }

                query = query.Where(x =>
                        (
                        (!string.IsNullOrEmpty($"{x.User.FirstName} {x.User.LastName}") && $"{x.User.FirstName} {x.User.LastName}".Contains(searchText))
                        || (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                     || (!string.IsNullOrEmpty(x.TimeFunctionStartDate.ToString("yyyy/MMMM/dddd HH:mm")) && x.TimeFunctionStartDate.ToString("yyyy/MMMM/dddd HH:mm").Contains(searchText)) 
                     || (!string.IsNullOrEmpty(x.TimeFunctionEndDate.ToString("yyyy/MMMM/dddd HH:mm")) && x.TimeFunctionEndDate.ToString("yyyy/MMMM/dddd HH:mm").Contains(searchText)) 
                        ));

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

        public async Task<RowResultObject<TimeFunction>> GetTimeFunctionByIdAsync(long TimeFunctionId)
        {
            RowResultObject<TimeFunction> result = new RowResultObject<TimeFunction>();
            try
            {
                result.Result = await _context.TimeFunctions.Include(x=> x.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == TimeFunctionId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTimeFunctionAsync(TimeFunction TimeFunction)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.TimeFunctions.Remove(TimeFunction);
                await _context.SaveChangesAsync();
                result.ID = TimeFunction.ID;
                _context.Entry(TimeFunction).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveTimeFunctionAsync(long TimeFunctionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var TimeFunction = await GetTimeFunctionByIdAsync(TimeFunctionId);
                result = await RemoveTimeFunctionAsync(TimeFunction.Result);
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