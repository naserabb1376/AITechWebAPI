using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using NobatPlusDATA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Services
{
    public class GroupChatMessageRep : IGroupChatMessageRep
    {

        private AITechContext _context;
        public GroupChatMessageRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddGroupChatMessageAsync(GroupChatMessage GroupChatMessage)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.GroupChatMessages.AddAsync(GroupChatMessage);
                await _context.SaveChangesAsync();
                result.ID = GroupChatMessage.ID;
                _context.Entry(GroupChatMessage).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> EditGroupChatMessageAsync(GroupChatMessage GroupChatMessage)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.GroupChatMessages.Update(GroupChatMessage);
                await _context.SaveChangesAsync();
                result.ID = GroupChatMessage.ID;
                _context.Entry(GroupChatMessage).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> ExistGroupChatMessageAsync(long GroupChatMessageId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.GroupChatMessages
                .AsNoTracking()
                .AnyAsync(x => x.ID == GroupChatMessageId);
                result.ID = GroupChatMessageId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<ListResultObject<GroupChatMessage>> GetAllGroupChatMessagesAsync(long GroupId = 0, long SenderUserId = 0, long ReplyToMessageId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<GroupChatMessage> results = new ListResultObject<GroupChatMessage>();
            try
            {
                var canAccess = await CanAccessGroupChatAsync(GroupId, SenderUserId);
                if (!canAccess.Status)
                    throw new UnauthorizedAccessException("شما به این گروه چت دسترسی ندارید");


                IQueryable<GroupChatMessage> query = _context.GroupChatMessages
                        .AsNoTracking().Include(x => x.SenderUser).Include(x=> x.Group).Include(x=> x.ReplyToMessage);

                if (SenderUserId == 0)
                {
                    query = query.Where(x=> x.SenderUserId == SenderUserId);
                }

                if (GroupId == 0)
                {
                    query = query.Where(x => x.GroupId == GroupId);
                }
                if (ReplyToMessageId == 0)
                {
                    query = query.Where(x => x.ReplyToMessageId == ReplyToMessageId);
                }

                query = query.Where(x =>
                            (!string.IsNullOrEmpty(x.Group.Name) && x.Group.Name.Contains(searchText)) ||
                            (!string.IsNullOrEmpty(x.SenderUser.FirstName) && x.SenderUser.FirstName.Contains(searchText)) ||
                            (!string.IsNullOrEmpty(x.SenderUser.LastName) && x.SenderUser.LastName.Contains(searchText)) ||
                            (!string.IsNullOrEmpty(x.SentAt.ToString()) && x.SentAt.ToString().Contains(searchText)) ||
                            (x.EditedAt.HasValue && x.EditedAt.Value.ToString().Contains(searchText)) ||
                            (x.DeletedAt.HasValue && x.DeletedAt.Value.ToString().Contains(searchText)) ||
                            (x.CreateDate.HasValue && x.CreateDate.Value.ToString().Contains(searchText)) ||
                            (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString().Contains(searchText))
                        );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;

        }

        public async Task<RowResultObject<GroupChatMessage>> GetGroupChatMessageByIdAsync(long GroupChatMessageId)
        {
            RowResultObject<GroupChatMessage> result = new RowResultObject<GroupChatMessage>();
            try
            {
                result.Result = await _context.GroupChatMessages
                .AsNoTracking().Include(x => x.SenderUser).Include(x => x.Group).Include(x => x.ReplyToMessage)
                .SingleOrDefaultAsync(x => x.ID == GroupChatMessageId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> RemoveGroupChatMessageAsync(GroupChatMessage GroupChatMessage)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.GroupChatMessages.Remove(GroupChatMessage);
                await _context.SaveChangesAsync();
                result.ID = GroupChatMessage.ID;
                _context.Entry(GroupChatMessage).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> RemoveGroupChatMessageAsync(long GroupChatMessageId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var GroupChatMessage = await GetGroupChatMessageByIdAsync(GroupChatMessageId);
                result = await RemoveGroupChatMessageAsync(GroupChatMessage.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<BitResultObject> CanAccessGroupChatAsync(long GroupId, long UserId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                // استاد گروه
                var isTeacher = await _context.Groups
                    .AsNoTracking()
                    .AnyAsync(g => g.ID == GroupId && g.TeacherId == UserId);


                // دانش‌آموز عضو گروه (UserGroup)
                var isStudent = await _context.UserGroups
                    .AsNoTracking()
                    .AnyAsync(ug => ug.GroupId == GroupId && ug.UserId == UserId);

                result.ID = 0;
                result.Status = isTeacher || isStudent;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;

        }

        public async Task<RowResultObject<GroupChatMessage>> SendMessageAsync(long groupId, long senderUserId, SendGroupMessageRequest request)
        {
            RowResultObject<GroupChatMessage> result = new RowResultObject<GroupChatMessage>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Text))
                    throw new ArgumentException("متن پیام را وارد کنید");

                var canAccess = await CanAccessGroupChatAsync(groupId, senderUserId);
                if (!canAccess.Status)
                    throw new UnauthorizedAccessException("شما امکان ارسال پیام در این گروه را ندارید");

                // اگر ReplyToMessageId داده شده، مطمئن شو پیام مربوط به همین گروه است
                if (request.ReplyToMessageId.HasValue)
                {
                    var okReply = await _context.GroupChatMessages
                        .AsNoTracking()
                        .AnyAsync(m => m.ID == request.ReplyToMessageId.Value && m.GroupId == groupId);

                    if (!okReply)
                        throw new InvalidOperationException("ارسال پاسخ برای این پیام مقدور نیست");
                }

                var message = new GroupChatMessage
                {
                    GroupId = groupId,
                    SenderUserId = senderUserId,
                    Text = request.Text.Trim(),
                    SentAt = DateTime.UtcNow,
                    ReplyToMessageId = request.ReplyToMessageId
                };
                var addResult = await AddGroupChatMessageAsync(message);
                if (!addResult.Status)
                {
                    result.Status = addResult.Status;
                    result.ErrorMessage = addResult.ErrorMessage;
                }
                else
                {
                    var GetGroupChat = await GetGroupChatMessageByIdAsync(message.ID);
                    if (!GetGroupChat.Status)
                    {
                        result.Status = GetGroupChat.Status;
                        result.ErrorMessage = GetGroupChat.ErrorMessage;
                    }
                    else
                    {
                        result.Result = GetGroupChat.Result;
                    }
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
