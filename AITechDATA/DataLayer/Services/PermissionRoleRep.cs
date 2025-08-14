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
using System.Runtime.ConstrainedExecution;

namespace AITechDATA.DataLayer.Services
{
    public class PermissionRoleRep : IPermissionRoleRep
    {
        private AiITechContext _context;

        public PermissionRoleRep(AiITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddPermissionRolesAsync(List<PermissionRole> PermissionRoles)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.PermissionRoles.AddRangeAsync(PermissionRoles);
                await _context.SaveChangesAsync();
                result.ID = PermissionRoles.FirstOrDefault().ID;
                foreach (var permissionRole in PermissionRoles)
                {
                    _context.Entry(permissionRole).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditPermissionRolesAsync(List<PermissionRole> PermissionRoles)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PermissionRoles.UpdateRange(PermissionRoles);
                await _context.SaveChangesAsync();
                result.ID = PermissionRoles.FirstOrDefault().ID;
                foreach (var permissionRole in PermissionRoles)
                {
                    _context.Entry(permissionRole).State = EntityState.Detached;
                }
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

        public async Task<ListResultObject<PermissionRole>> GetAllPermissionRolesAsync(long RoleId = 0, long PerrmissionId = 0, string permissionType = "", int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<PermissionRole> results = new ListResultObject<PermissionRole>();
            try
            {
                var query = _context.PermissionRoles.Include(x =>x.Role).Include(x=> x.Permission)
                    .AsNoTracking()
                    .Where(x =>
                        x.Permission.Name.ToString().Contains(searchText) ||
                        x.Permission.Routename.ToString().Contains(searchText) ||
                        x.Role.Name.ToString().Contains(searchText)
                    );

                if (PerrmissionId > 0)
                {
                    query = query.Where(x => x.PerrmissionId == PerrmissionId);
                }

                if (RoleId > 0)
                {
                    query = query.Where(x => x.RoleId == RoleId);
                }

                if (!string.IsNullOrEmpty(permissionType))
                {
                    query = query.Where(x => x.Permission.PermissionType == permissionType);
                }


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

        public async Task<BitResultObject> RemovePermissionRolesAsync(List<PermissionRole> PermissionRoles)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PermissionRoles.RemoveRange(PermissionRoles);
                await _context.SaveChangesAsync();
                result.ID = PermissionRoles.FirstOrDefault().ID;
                foreach (var permissionRole in PermissionRoles)
                {
                    _context.Entry(permissionRole).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePermissionRolesAsync(List<long> PermissionRoleIds)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var PermissionRolesToRemove = new List<PermissionRole>();

                foreach (var PermissionRoleId in PermissionRoleIds)
                {
                    var PermissionRole = await GetPermissionRoleByIdAsync(PermissionRoleId);
                    if (PermissionRole.Result != null)
                    {
                        PermissionRolesToRemove.Add(PermissionRole.Result);
                    }
                }

                if (PermissionRolesToRemove.Any())
                {
                    result = await RemovePermissionRolesAsync(PermissionRolesToRemove);
                }
                else
                {
                    result.Status = false;
                    result.ErrorMessage = "No matching PermissionRoles found to remove.";
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