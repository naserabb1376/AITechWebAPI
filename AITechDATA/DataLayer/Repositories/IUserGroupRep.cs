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
        Task<ListResultObject<UserGroup>> GetAllUserGroupsAsync(long userId=0,long groupId=0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<UserGroup>> GetUserGroupByIdAsync(long UserGroupId);


        Task<BitResultObject> AddUserGroupsAsync(List<UserGroup> UserGroups);

        Task<BitResultObject> EditUserGroupsAsync(List<UserGroup> UserGroups);

        Task<BitResultObject> RemoveUserGroupsAsync(List<UserGroup> UserGroups);

        Task<BitResultObject> RemoveUserGroupsAsync(List<long> UserGroupIds);

        Task<BitResultObject> ExistUserGroupAsync(long UserGroupId);
    }
}