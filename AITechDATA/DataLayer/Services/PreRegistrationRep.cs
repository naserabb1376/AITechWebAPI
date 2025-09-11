using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Services
{
    public class PreRegistrationRep : IPreRegistrationRep
    {
        private AITechContext _context;

        public PreRegistrationRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddPreRegistrationAsync(PreRegistration preRegistration)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.PreRegistrations.AddAsync(preRegistration);
                await _context.SaveChangesAsync();
                result.ID = preRegistration.ID;
                _context.Entry(preRegistration).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditPreRegistrationAsync(PreRegistration preRegistration)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PreRegistrations.Update(preRegistration);
                await _context.SaveChangesAsync();
                result.ID = preRegistration.ID;
                _context.Entry(preRegistration).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistPreRegistrationAsync(long preRegistrationId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.PreRegistrations
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == preRegistrationId);
                result.ID = preRegistrationId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<PreRegistration>> GetAllPreRegistrationsAsync(long foreignkeyId = 0, string entityType = "", int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<PreRegistration> results = new ListResultObject<PreRegistration>();
            try
            {
                var query = _context.PreRegistrations.AsNoTracking();

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(x => x.EntityType == entityType);

                if (foreignkeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == foreignkeyId);

                query = query.Where(x =>
                        ((!string.IsNullOrEmpty(x.FirstName) && x.FirstName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Contains(searchText)))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.RegistrationDate)
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

        public async Task<RowResultObject<PreRegistration>> GetPreRegistrationByIdAsync(long preRegistrationId)
        {
            RowResultObject<PreRegistration> result = new RowResultObject<PreRegistration>();
            try
            {
                result.Result = await _context.PreRegistrations
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == preRegistrationId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePreRegistrationAsync(PreRegistration preRegistration)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PreRegistrations.Remove(preRegistration);
                await _context.SaveChangesAsync();
                result.ID = preRegistration.ID;
                _context.Entry(preRegistration).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePreRegistrationAsync(long preRegistrationId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var preRegistration = await GetPreRegistrationByIdAsync(preRegistrationId);
                result = await RemovePreRegistrationAsync(preRegistration.Result);
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