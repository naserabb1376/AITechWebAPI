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
    public class LoginMethodRep : ILoginMethodRep
    {
        private AiITechContext _context;

        public LoginMethodRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddLoginMethodAsync(LoginMethod loginMethod)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.LoginMethods.AddAsync(loginMethod);
                await _context.SaveChangesAsync();
                result.ID = loginMethod.ID;
                _context.Entry(loginMethod).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditLoginMethodAsync(LoginMethod loginMethod)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.LoginMethods.Update(loginMethod);
                await _context.SaveChangesAsync();
                result.ID = loginMethod.ID;
                _context.Entry(loginMethod).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistLoginMethodAsync(long loginMethodId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.LoginMethods
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == loginMethodId);
                result.ID = loginMethodId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<LoginMethod>> GetAllLoginMethodsAsync(long userId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="")
        {
            ListResultObject<LoginMethod> results = new ListResultObject<LoginMethod>();
            try
            {
                var query = _context.LoginMethods
                    .AsNoTracking()
                    .Where(x =>
                        (userId > 0 && x.UserId == userId) ||
                       ( (!string.IsNullOrEmpty(x.Method) && x.Method.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Token) && x.Token.Contains(searchText))
                    ));

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ExpirationDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<LoginMethod>> GetLoginMethodByIdAsync(long loginMethodId)
        {
            RowResultObject<LoginMethod> result = new RowResultObject<LoginMethod>();
            try
            {
                result.Result = await _context.LoginMethods
                    .AsNoTracking()
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.ID == loginMethodId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveLoginMethodAsync(LoginMethod loginMethod)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.LoginMethods.Remove(loginMethod);
                await _context.SaveChangesAsync();
                result.ID = loginMethod.ID;
                _context.Entry(loginMethod).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveLoginMethodAsync(long loginMethodId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var loginMethod = await GetLoginMethodByIdAsync(loginMethodId);
                result = await RemoveLoginMethodAsync(loginMethod.Result);
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