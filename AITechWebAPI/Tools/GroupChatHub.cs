using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace AITechWebAPI.Tools
{
    [Authorize]
    public class GroupChatHub : Hub
    {
        private readonly IGroupChatMessageRep _chatService;
        private readonly IUserRep _userRep;
        private readonly IGroupChatReadStateRep _seenService;

        public GroupChatHub(IGroupChatMessageRep chatService,IUserRep userRep,IGroupChatReadStateRep seenService)
        {
            _chatService = chatService;
            _userRep = userRep;
            _seenService = seenService;
        }

        private static string Room(long groupId) => $"group:{groupId}";

        public override async Task OnConnectedAsync()
        {
            // اینجا فعلاً Join خودکار نمی‌کنیم، چون کاربر ممکنه چند گروه داشته باشه
            await base.OnConnectedAsync();
        }

        public async Task JoinGroup(long groupId)
        {
            var userId = Context.User!.GetCurrentUserId();
            long roleId = Context.User!.GetCurrentRoleId();

            var canAccess = await _chatService.CanAccessGroupChatAsync(groupId, userId,roleId);
            if (!canAccess)
                throw new HubException("Access denied to this group chat.");

            await Groups.AddToGroupAsync(Context.ConnectionId, Room(groupId));

            // به خود کاربر بگو Join شده (اختیاری)
            await Clients.Caller.SendAsync("Joined", new { groupId });
        }

        public async Task LeaveGroup(long groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Room(groupId));
            await Clients.Caller.SendAsync("Left", new { groupId });
        }

        public async Task SendMessage(long groupId, SendGroupMessageRequest request)
        {
            try
            {
                var userId = Context.User!.GetCurrentUserId();
                long roleId = Context.User!.GetCurrentRoleId();
                var created = await _chatService.SendMessageAsync(roleId,groupId, userId, request);
                await Clients.Group($"group:{groupId}").SendAsync("MessageReceived", created);
            }
            catch (Exception ex)
            {
                // این باعث میشه خطا به کلاینت هم برگرده
                throw new HubException(ex.Message);
            }
        }

        public async Task EditMessage(long groupId, long messageId, EditGroupMessageRequest request)
        {
            var userId = Context.User!.GetCurrentUserId();
            long roleId = Context.User!.GetCurrentRoleId();

            var updated = await _chatService.EditMessageAsync(roleId,groupId, messageId, userId, request);

            await Clients.Group(Room(groupId)).SendAsync("MessageEdited", updated);
        }

        public async Task DeleteMessage(long groupId, long messageId)
        {
            var userId = Context.User!.GetCurrentUserId();
            long roleId = Context.User!.GetCurrentRoleId();
            await _chatService.SoftDeleteMessageAsync(roleId, groupId, messageId, userId);

            await Clients.Group(Room(groupId)).SendAsync("MessageDeleted", new { groupId, messageId });
        }

        public async Task Seen(long groupId, long lastReadMessageId)
        {
            var userId = Context.User!.GetCurrentUserId();

            await _seenService.MarkAsSeenAsync(groupId, userId, lastReadMessageId);

            await Clients.Group($"group:{groupId}")
                .SendAsync("SeenUpdated", new { groupId, userId, lastReadMessageId });
        }

        public async Task Typing(long groupId, bool isTyping)
        {
            var userId = Context.User!.GetCurrentUserId();

            // نام کاربر را از DB یا هر سرویس کاربر بگیر
            // (بهترین: یک سرویس IUserService یا از DbContext استفاده کنی)
            var u = await _userRep.GetUserByIdAsync(userId);

            var name = (u == null)
                ? $"User#{userId}"
                : ((u.Result.FirstName + " " + u.Result.LastName).Trim().Length > 0
                    ? (u.Result.FirstName + " " + u.Result.LastName).Trim()
                    : (u.Result.Username ?? $"User#{userId}"));

            await Clients.OthersInGroup($"group:{groupId}").SendAsync("Typing", new
            {
                groupId,
                userId,
                name,
                isTyping
            });
        }


        public async Task AttachFile(long groupId, long messageId, AttachFileToMessageRequest request)
        {
            var userId = Context.User!.GetCurrentUserId();
            long roleId = Context.User!.GetCurrentRoleId();

            var updated = await _chatService.AttachFileToMessageAsync(roleId, groupId, messageId, userId, request);

            await Clients.Group($"group:{groupId}")
                .SendAsync("MessageAttachmentUpdated", updated);
        }

    }
}
