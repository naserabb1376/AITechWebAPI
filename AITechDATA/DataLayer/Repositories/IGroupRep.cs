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
        Task<ListResultObject<GroupDto>> GetAllGroupsAsync(long ClientUserId, long ClientRoleId, long studentId = 0,long courseId = 0,long teacherId=0, string groupStatus = "", string groupType = "", int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<GroupDto>> GetGroupByIdAsync(long groupId, long ClientUserId = 0, long ClientRoleId = 0);

        Task<BitResultObject> AddGroupAsync(Group group);

        Task<BitResultObject> EditGroupAsync(Group group);

        Task<BitResultObject> RemoveGroupAsync(Group group);

        Task<BitResultObject> RemoveGroupAsync(long groupId);

        Task<BitResultObject> ExistGroupAsync(long groupId);
    }
}