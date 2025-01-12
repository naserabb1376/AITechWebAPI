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
    public class PermissionRoleRep : IPermissionRoleRep
    {
        private AiITechContext _context;

        public PermissionRoleRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddPermissionRoleAsync(PermissionRole PermissionRole)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.PermissionRoles.AddAsync(PermissionRole);
                await _context.SaveChangesAsync();
                result.ID = PermissionRole.ID;
                _context.Entry(PermissionRole).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditPermissionRoleAsync(PermissionRole PermissionRole)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PermissionRoles.Update(PermissionRole);
                await _context.SaveChangesAsync();
                result.ID = PermissionRole.ID;
                _context.Entry(PermissionRole).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistPermissionRoleAsync(long PermissionRoleId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.PermissionRoles
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == PermissionRoleId);
                result.ID = PermissionRoleId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<PermissionRole>> GetAllPermissionRolesAsync(long RoleId = 0, long PerrmissionId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<PermissionRole> results = new ListResultObject<PermissionRole>();
            try
            {
                var query = _context.PermissionRoles
                    .AsNoTracking()
                    .Where(x =>
                        (RoleId > 0 && x.RoleId == RoleId) ||
                        (PerrmissionId > 0 && x.PerrmissionId == PerrmissionId) ||
                        x.Permission.Name.ToString().Contains(searchText) ||
                        x.Role.Name.ToString().Contains(searchText)
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.ID)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.Role)
                    .Include(x => x.Permission)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<PermissionRole>> GetPermissionRoleByIdAsync(long PermissionRoleId)
        {
            RowResultObject<PermissionRole> result = new RowResultObject<PermissionRole>();
            try
            {
                result.Result = await _context.PermissionRoles
                    .AsNoTracking()
                    .Include(x => x.Permission)
                    .Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.ID == PermissionRoleId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePermissionRoleAsync(PermissionRole PermissionRole)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PermissionRoles.Remove(PermissionRole);
                await _context.SaveChangesAsync();
                result.ID = PermissionRole.ID;
                _context.Entry(PermissionRole).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePermissionRoleAsync(long PermissionRoleId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var PermissionRole = await GetPermissionRoleByIdAsync(PermissionRoleId);
                result = await RemovePermissionRoleAsync(PermissionRole.Result);
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