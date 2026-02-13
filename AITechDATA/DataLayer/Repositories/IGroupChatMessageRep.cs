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
        public Task<ListResultObject<GroupChatMessage>> GetAllGroupChatMessagesAsync(long roleId = 0, long GroupId = 0, long SenderUserId = 0, long ReplyToMessageId = 0, bool withDeleted = false, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");
        public Task<RowResultObject<GroupChatMessage>> GetGroupChatMessageByIdAsync(long GroupChatMessageId);
        public Task<BitResultObject> AddGroupChatMessageAsync(GroupChatMessage GroupChatMessage);
        public Task<BitResultObject> EditGroupChatMessageAsync(GroupChatMessage GroupChatMessage);
        public Task<BitResultObject> RemoveGroupChatMessageAsync(GroupChatMessage GroupChatMessage);
        public Task<BitResultObject> RemoveGroupChatMessageAsync(long GroupChatMessageId);
        public Task<BitResultObject> ExistGroupChatMessageAsync(long GroupChatMessageId);

        // Messenger 
        public Task<bool> CanAccessGroupChatAsync(long groupId, long userId,long roleId);
        public Task<GroupChatMessageDto> SendMessageAsync(long roleId, long groupId, long senderUserId, SendGroupMessageRequest request);
        public Task<List<GroupChatMessageDto>> GetMessagesAsync(long roleId, long groupId, long userId, int pageIndex, int pageSize);
        public Task<GroupChatMessageDto> EditMessageAsync(long roleId, long groupId, long messageId, long userId, EditGroupMessageRequest request);
        public Task SoftDeleteMessageAsync(long roleId, long groupId, long messageId, long userId);

        // Attachment

        Task<GroupChatMessageDto> AttachFileToMessageAsync(long roleId, long groupId, long messageId, long userId, AttachFileToMessageRequest request);

    }
}
