using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IPermissionRep
    {
        Task<ListResultObject<Permission>> GetAllPermissionsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<Permission>> GetPermissionByIdAsync(long permissionId);

        Task<BitResultObject> AddPermissionAsync(Permission permission);

        Task<BitResultObject> EditPermissionAsync(Permission permission);

        Task<BitResultObject> RemovePermissionAsync(Permission permission);

        Task<BitResultObject> RemovePermissionAsync(long permissionId);

        Task<BitResultObject> ExistPermissionAsync(long permissionId);
    }
}