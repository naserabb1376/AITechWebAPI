using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IGroupRep
    {
        Task<ListResultObject<Group>> GetAllGroupsAsync(long userId = 0,long courseId = 0, string groupStatus = "", int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<Group>> GetGroupByIdAsync(long groupId);

        Task<BitResultObject> AddGroupAsync(Group group);

        Task<BitResultObject> EditGroupAsync(Group group);

        Task<BitResultObject> RemoveGroupAsync(Group group);

        Task<BitResultObject> RemoveGroupAsync(long groupId);

        Task<BitResultObject> ExistGroupAsync(long groupId);
    }
}