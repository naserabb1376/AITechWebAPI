using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IUserGroupRep
    {
        Task<ListResultObject<UserGroup>> GetAllUserGroupsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<UserGroup>> GetUserGroupByIdAsync(long UserGroupId);

        Task<BitResultObject> AddUserGroupAsync(UserGroup UserGroup);

        Task<BitResultObject> EditUserGroupAsync(UserGroup UserGroup);

        Task<BitResultObject> RemoveUserGroupAsync(UserGroup UserGroup);

        Task<BitResultObject> RemoveUserGroupAsync(long UserGroupId);

        Task<BitResultObject> ExistUserGroupAsync(long UserGroupId);
    }
}