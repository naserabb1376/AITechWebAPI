using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using NobatPlusDATA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IGroupChatReadStateRep
    {
        public Task<ListResultObject<GroupChatReadState>> GetAllGroupChatReadStatesAsync(long GroupId = 0, long UserId = 0, long RoleId = 0, long LastReadMessageId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");
        public Task<RowResultObject<GroupChatReadState>> GetGroupChatReadStateByIdAsync(long GroupChatReadStateId);
        public Task<BitResultObject> AddGroupChatReadStateAsync(GroupChatReadState GroupChatReadState);
        public Task<BitResultObject> EditGroupChatReadStateAsync(GroupChatReadState GroupChatReadState);
        public Task<BitResultObject> RemoveGroupChatReadStateAsync(GroupChatReadState GroupChatReadState);
        public Task<BitResultObject> RemoveGroupChatReadStateAsync(long GroupChatReadStateId);
        public Task<BitResultObject> ExistGroupChatReadStateAsync(long GroupChatReadStateId);

        // Seen
        public Task MarkAsSeenAsync(long groupId, long userId, long RoleId, long lastReadMessageId);
        public Task<GroupChatReadStateDto> GetReadStateAsync(long groupId, long userId, long roleId);
        public Task<List<GroupMemberReadStateDto>> GetGroupReadStatesAsync(long groupId, long userId, long roleId);

    }
}
