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
    public interface IGroupChatMessageRep
    {
        public Task<ListResultObject<GroupChatMessage>> GetAllGroupChatMessagesAsync(long GroupId = 0, long SenderUserId = 0, long ReplyToMessageId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");
        public Task<RowResultObject<GroupChatMessage>> GetGroupChatMessageByIdAsync(long GroupChatMessageId);
        public Task<BitResultObject> AddGroupChatMessageAsync(GroupChatMessage GroupChatMessage);
        public Task<BitResultObject> EditGroupChatMessageAsync(GroupChatMessage GroupChatMessage);
        public Task<BitResultObject> RemoveGroupChatMessageAsync(GroupChatMessage GroupChatMessage);
        public Task<BitResultObject> RemoveGroupChatMessageAsync(long GroupChatMessageId);
        public Task<BitResultObject> ExistGroupChatMessageAsync(long GroupChatMessageId);
        public Task<BitResultObject> CanAccessGroupChatAsync(long GroupId, long UserId);
        public Task<RowResultObject<GroupChatMessage>> SendMessageAsync(long groupId, long senderUserId, SendGroupMessageRequest request);

    }
}
