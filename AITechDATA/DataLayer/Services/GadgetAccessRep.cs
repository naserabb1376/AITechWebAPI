using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;

namespace AITechDATA.DataLayer.Services
{
    public class GadgetAccessRep : IGadgetAccessRep
    {
        private AITechContext _context;

        public GadgetAccessRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<RowResultObject<GadgetAccess>> AddGadgetAccessAsync(GadgetAccess GadgetAccess)
        {
            RowResultObject<GadgetAccess> result = new RowResultObject<GadgetAccess>();
            try
            {
                if (string.IsNullOrEmpty(GadgetAccess.AccessUsername))
                {
                    var gadgetCount = await _context.GadgetAccesses.CountAsync() +1;
                    GadgetAccess.AccessUsername = $"TempUser{gadgetCount}";
                    var existUserName = await _context.GadgetAccesses.AnyAsync(x => x.AccessUsername == GadgetAccess.AccessUsername);
                    while (existUserName)
                    {
                        gadgetCount++;
                        GadgetAccess.AccessUsername = $"TempUser{gadgetCount}";
                        existUserName = await _context.GadgetAccesses.AnyAsync(x => x.AccessUsername == GadgetAccess.AccessUsername);
                    }
                }
                if (string.IsNullOrEmpty(GadgetAccess.AccessPassword))
                {
                    GadgetAccess.AccessPassword = GadgetAccess.AccessUsername;
                }
                GadgetAccess.AccessPassword = GadgetAccess.AccessPassword.ToHash();

                await _context.GadgetAccesses.AddAsync(GadgetAccess);
                await _context.SaveChangesAsync();
                _context.Entry(GadgetAccess).State = EntityState.Detached;

                result.Result = GadgetAccess;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<RowResultObject<GadgetAccess>> EditGadgetAccessAsync(GadgetAccess GadgetAccess)
        {
            RowResultObject<GadgetAccess> result = new RowResultObject<GadgetAccess>();
            try
            {
                if (string.IsNullOrEmpty(GadgetAccess.AccessUsername))
                {
                    var gadgetCount = await _context.GadgetAccesses.CountAsync() + 1;
                    GadgetAccess.AccessUsername = $"TempUser{gadgetCount}";
                    var existUserName = await _context.GadgetAccesses.AnyAsync(x => x.AccessUsername == GadgetAccess.AccessUsername);
                    while (existUserName)
                    {
                        gadgetCount++;
                        GadgetAccess.AccessUsername = $"TempUser{gadgetCount}";
                        existUserName = await _context.GadgetAccesses.AnyAsync(x => x.AccessUsername == GadgetAccess.AccessUsername);
                    }
                }
                if (string.IsNullOrEmpty(GadgetAccess.AccessPassword))
                {
                    GadgetAccess.AccessPassword = GadgetAccess.AccessUsername;
                }
                GadgetAccess.AccessPassword = GadgetAccess.AccessPassword.ToHash();

                _context.GadgetAccesses.Update(GadgetAccess);
                await _context.SaveChangesAsync();
                _context.Entry(GadgetAccess).State = EntityState.Detached;

                result.Result= GadgetAccess;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistGadgetAccessAsync(long GadgetAccessId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.GadgetAccesses
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == GadgetAccessId);
                result.ID = GadgetAccessId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<GadgetAccess>> GetAllGadgetAccessesAsync(
      string accessUserName = "", string gadgetKey = "", int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<GadgetAccess> results = new ListResultObject<GadgetAccess>();

            try
            {
                var query = _context.GadgetAccesses.AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن
                
                if (!string.IsNullOrEmpty(accessUserName))
                    query = query.Where(x => x.AccessUsername == accessUserName);

                if (!string.IsNullOrEmpty(gadgetKey))
                    query = query.Where(x => x.GadgetKey == gadgetKey);

                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.GadgetKey) && x.GadgetKey.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.GadgetDescription) && x.GadgetDescription.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.GadgetUrl) && x.GadgetUrl.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.AccessUsername) && x.AccessUsername.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.AccessPassword) && x.AccessPassword.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderByDescending(x => x.CreateDate)
                    .SortBy(sortQuery)
                    .ToPaging(pageIndex, pageSize)
                    //.Include(x => x.Assignment) // در صورت نیاز بازکنید
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }


        public async Task<ListResultObject<AccessableGadgetsDto>> GetAcessableGadgetsAsync(string accessUsername, string accessPassword, string gadgetKey = "")
        {
            ListResultObject<AccessableGadgetsDto> results = new ListResultObject<AccessableGadgetsDto>();

            try
            {
                var query = _context.GadgetAccesses.AsNoTracking().AsQueryable();
                var theUser = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == accessUsername.ToLower()
                && x.PasswordHash == accessPassword.ToHash());
                var userExist = theUser != null && theUser.RoleId > 2;

                if (!userExist)
                    query = query.Where(x => x.AccessUsername.ToLower() == accessUsername.ToLower() && x.AccessPassword == accessPassword.ToHash()
                   && (x.AccessStartDate == null || x.AccessStartDate >= DateTime.Now)
                   && (x.AccessEndDate <= DateTime.Now)
                    );

                if (!string.IsNullOrEmpty(gadgetKey))
                {
                    query = query.Where(x => x.GadgetKey.ToLower() == gadgetKey.ToLower());
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, 0);

                results.Results = await query
                    .OrderByDescending(x => x.CreateDate)
                    .Select(x=> new AccessableGadgetsDto()
                    {
                        GadgetKey = x.GadgetKey,
                        GadgetDescription = x.GadgetDescription,
                        GadgetUrl = x.GadgetUrl,
                    })
                    .ToPaging(1, 0)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }

        public async Task<RowResultObject<GadgetAccess>> GetGadgetAccessByIdAsync(long GadgetAccessId)
        {
            RowResultObject<GadgetAccess> result = new RowResultObject<GadgetAccess>();
            try
            {
                result.Result = await _context.GadgetAccesses
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == GadgetAccessId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }


        public async Task<BitResultObject> RemoveGadgetAccessAsync(GadgetAccess GadgetAccess)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.GadgetAccesses.Remove(GadgetAccess);
                await _context.SaveChangesAsync();
                result.ID = GadgetAccess.ID;
                _context.Entry(GadgetAccess).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveGadgetAccessAsync(long GadgetAccessId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var GadgetAccess = await GetGadgetAccessByIdAsync(GadgetAccessId);
                result = await RemoveGadgetAccessAsync(GadgetAccess.Result);
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