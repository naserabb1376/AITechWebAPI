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
    public class PermissionRep : IPermissionRep
    {
        private AiITechContext _context;

        public PermissionRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddPermissionAsync(Permission permission)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();
                result.ID = permission.ID;
                _context.Entry(permission).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditPermissionAsync(Permission permission)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Permissions.Update(permission);
                await _context.SaveChangesAsync();
                result.ID = permission.ID;
                _context.Entry(permission).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistPermissionAsync(long permissionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Permissions
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == permissionId);
                result.ID = permissionId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Permission>> GetAllPermissionsAsync(long roleId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<Permission> results = new ListResultObject<Permission>();
            try
            {
                IQueryable<Permission> query;
                if (roleId > 0)
                {
                    query = _context.PermissionRoles.Where(x => x.RoleId == roleId).Select(x => x.Permission)
                   .AsNoTracking()
                   .Where(x =>
                        (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                   );
                }
                else
                {
                    query = _context.Permissions
                  .AsNoTracking()
                  .Where(x =>
                      (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)) ||
                      (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                  );
                }

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    //.Include(x => x.PermissionRoles)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<Permission>> GetPermissionByIdAsync(long permissionId)
        {
            RowResultObject<Permission> result = new RowResultObject<Permission>();
            try
            {
                result.Result = await _context.Permissions
                    .AsNoTracking()
                    .Include(x => x.PermissionRoles)
                    .SingleOrDefaultAsync(x => x.ID == permissionId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePermissionAsync(Permission permission)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();
                result.ID = permission.ID;
                _context.Entry(permission).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePermissionAsync(long permissionId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var permission = await GetPermissionByIdAsync(permissionId);
                result = await RemovePermissionAsync(permission.Result);
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