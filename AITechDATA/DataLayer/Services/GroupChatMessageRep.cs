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

        public async Task<ListResultObject<GroupChatMessage>> GetAllGroupChatMessagesAsync(long roleId = 0, long GroupId = 0, long SenderUserId = 0, long ReplyToMessageId = 0,bool withDeleted = false, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<GroupChatMessage> results = new ListResultObject<GroupChatMessage>();
            try
            {
                if (GroupId > 0 && SenderUserId > 0)
                {
                    var canAccess = await CanAccessGroupChatAsync(GroupId, SenderUserId,roleId);
                    if (!canAccess)
                    {
                        results.ErrorMessage = "شما به این گروه چت دسترسی ندارید";
                        results.Status = false;
                        return results;
                    }

                }


                IQueryable<GroupChatMessage> query = _context.GroupChatMessages
                        .AsNoTracking().Include(x => x.SenderUser).Include(x => x.ReplyToMessage).Include(x=> x.Group)
                        .ThenInclude(g=> g.Course).Include(x=> x.Group).ThenInclude(g=> g.Teacher)
                        .Include(x => x.SenderUser);

                if (!withDeleted)
                {
                    query = query.Where(x=> !x.IsDeleted);
                }

                if (SenderUserId > 0)
                {
                    query = query.Where(x=> x.SenderUserId == SenderUserId);
                }

                if (GroupId > 0)
                {
                    query = query.Where(x => x.GroupId == GroupId);
                }
                if (ReplyToMessageId > 0)
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
                results.Results = await query.OrderByDescending(x => x.SentAt)
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
                .AsNoTracking().Include(x => x.SenderUser).Include(x => x.ReplyToMessage).Include(x => x.Group)
                .ThenInclude(g => g.Course).Include(x => x.Group).ThenInclude(g => g.Teacher)
                .Include(x => x.SenderUser)
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


        // Messenger

        public async Task<bool> CanAccessGroupChatAsync(long groupId, long userId, long roleId)
        {
            var existGroup = await _context.Groups
               .AsNoTracking()
               .AnyAsync(g => g.ID == groupId);
            if (!existGroup) return false;

            // ادمین
            var isAdmin = roleId == 3 || roleId == 4 || roleId == 7;

            if (isAdmin) return true;

            // استاد گروه
            var isTeacher = await _context.Groups
                .AsNoTracking()
                .AnyAsync(g => g.ID == groupId && g.TeacherId == userId);

            if (isTeacher) return true;

            // دانش‌آموز عضو گروه (UserGroup)
            var isStudent = await _context.UserGroups
                .AsNoTracking()
                .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

            return isStudent;
        }

        public async Task<GroupChatMessageDto> SendMessageAsync(long roleId, long groupId, long senderUserId, SendGroupMessageRequest request)
        {
            //var since = DateTime.UtcNow.AddSeconds(-2);

            //var recentCount = await _context.GroupChatMessages.AsNoTracking()
            //    .CountAsync(m => m.GroupId == groupId &&
            //                     m.SenderUserId == senderUserId &&
            //                     m.SentAt >= since);

            //if (recentCount >= 3)
            //    throw new ArgumentException("لطفاً کمی آهسته‌تر پیام ارسال کنید");


            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("متن پیام را وارد کنید");

            var canAccess = await CanAccessGroupChatAsync(groupId, senderUserId, roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            // اگر ReplyToMessageId داده شده، مطمئن شو پیام مربوط به همین گروه است
            if (request.ReplyToMessageId.HasValue)
            {
                var okReply = await _context.GroupChatMessages
                    .AsNoTracking()
                    .AnyAsync(m => m.ID == request.ReplyToMessageId.Value && m.GroupId == groupId);

                if (!okReply)
                    throw new InvalidOperationException("پیام پیدا نشد");
            }

            var message = new GroupChatMessage
            {
                GroupId = groupId,
                SenderUserId = senderUserId,
                Text = request.Text.Trim(),
                SentAt = DateTime.Now.ToShamsi(),
                ReplyToMessageId = request.ReplyToMessageId
            };

            await AddGroupChatMessageAsync(message);  

            // خروجی DTO
            return await BuildDtoAsync(message.ID);
        }

        public async Task<List<GroupChatMessageDto>> GetMessagesAsync(long roleId, long groupId, long userId, int pageIndex, int pageSize)
        {
            var canAccess = await CanAccessGroupChatAsync(groupId, userId, roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            // جدیدترین‌ها اول، صفحه‌بندی
            var query = _context.GroupChatMessages
                .AsNoTracking()
                .Where(m => m.GroupId == groupId && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            // اگر User->Person داری، اینجا Join می‌کنیم
            var list = await (
                from m in query
                join u in _context.Users on m.SenderUserId equals u.ID
                select new GroupChatMessageDto
                {
                    Id = m.ID,
                    GroupId = m.GroupId,
                    SenderUserId = m.SenderUserId,
                    SenderName = u != null ? (u.FirstName + " " + u.LastName) : ("User#" + m.SenderUserId),
                    Text = m.Text,
                    SentAt = m.SentAt,
                    IsEdited = m.IsEdited,
                    EditedAt = m.EditedAt,
                    ReplyToMessageId = m.ReplyToMessageId,
                    AttachmentUrl = m.AttachmentUrl,
                    AttachmentName = m.AttachmentName,
                    AttachmentSize = m.AttachmentSize,
                    AttachmentType = m.AttachmentType
                }
            ).ToListAsync();

            return list;
        }

        public async Task<GroupChatMessageDto> EditMessageAsync(long roleId, long groupId, long messageId, long userId, EditGroupMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("متن پیام را وارد کنید");

            var canAccess = await CanAccessGroupChatAsync(groupId, userId, roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            var message = await _context.GroupChatMessages
                .FirstOrDefaultAsync(m => m.ID == messageId && m.GroupId == groupId);

            if (message == null)
                throw new KeyNotFoundException("پیام پیدا نشد");

            // فقط نویسنده خودش بتواند ادیت کند (سیاست پیش‌فرض)
            if (message.SenderUserId != userId)
                throw new UnauthorizedAccessException("You can only edit your own message.");

            message.Text = request.Text.Trim();
            message.IsEdited = true;
            message.EditedAt = DateTime.Now.ToShamsi();

            await EditGroupChatMessageAsync(message);

            return await BuildDtoAsync(message.ID);
        }

        public async Task SoftDeleteMessageAsync(long roleId, long groupId, long messageId, long userId)
        {
            var canAccess = await CanAccessGroupChatAsync(groupId, userId, roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            var message = await _context.GroupChatMessages
                .FirstOrDefaultAsync(m => m.ID == messageId && m.GroupId == groupId);

            if (message == null)
                throw new KeyNotFoundException("پیام پیدا نشد");

            // سیاست: فقط نویسنده یا استاد گروه بتواند حذف کند
            var isTeacher = await _context.Groups
                .AsNoTracking()
                .AnyAsync(g => g.ID == groupId && g.TeacherId == userId);

            if (message.SenderUserId != userId && !isTeacher)
                throw new UnauthorizedAccessException("شما دسترسی حذف این پیام را ندارید");

            // Soft delete
            message.IsDeleted = true;
            message.DeletedAt = DateTime.Now.ToShamsi();

            await EditGroupChatMessageAsync(message);
        }

        private async Task<GroupChatMessageDto> BuildDtoAsync(long messageId)
        {
            // اینجا هم با join اطلاعات فرستنده رو می‌گیریم
            var dto = await (
                from m in _context.GroupChatMessages.AsNoTracking()
                where m.ID == messageId
                join u in _context.Users on m.SenderUserId equals u.ID
                select new GroupChatMessageDto
                {
                    Id = m.ID,
                    GroupId = m.GroupId,
                    SenderUserId = m.SenderUserId,
                    SenderName = u != null ? (u.FirstName + " " + u.LastName) : ("User#" + m.SenderUserId),
                    Text = m.Text,
                    SentAt = m.SentAt,
                    IsEdited = m.IsEdited,
                    EditedAt = m.EditedAt,
                    ReplyToMessageId = m.ReplyToMessageId,
                    AttachmentUrl = m.AttachmentUrl,
                    AttachmentName = m.AttachmentName,
                    AttachmentSize = m.AttachmentSize,
                    AttachmentType = m.AttachmentType
                }
            ).FirstAsync();

            return dto;
        }

        // Attachment 

        public async Task<GroupChatMessageDto> AttachFileToMessageAsync(long roleId, long groupId, long messageId, long userId, AttachFileToMessageRequest request)
        {
            if (request == null)
                throw new ArgumentException("اطلاعات فایل نامعتبر است");

            if (string.IsNullOrWhiteSpace(request.AttachmentUrl))
                throw new ArgumentException("لینک فایل نامعتبر است");

            if (string.IsNullOrWhiteSpace(request.AttachmentType))
                throw new ArgumentException("نوع فایل مشخص نیست");

            var type = request.AttachmentType.Trim().ToLower();
            if (type != "images" && type != "files")
                throw new ArgumentException("نوع فایل فقط می‌تواند images یا files باشد");

            // دسترسی به گروه
            var canAccess = await CanAccessGroupChatAsync(groupId, userId, roleId);
            if (!canAccess)
                throw new UnauthorizedAccessException("شما به این گروه دسترسی ندارید");

            var message = await _context.GroupChatMessages
                .FirstOrDefaultAsync(m => m.ID == messageId && m.GroupId == groupId && !m.IsDeleted);

            if (message == null)
                throw new KeyNotFoundException("پیام پیدا نشد");

            // سیاست دسترسی:
            // فقط فرستنده پیام یا استاد گروه بتواند فایل اضافه کند
            var isTeacher = await _context.Groups.AsNoTracking()
                .AnyAsync(g => g.ID == groupId && g.TeacherId == userId);

            if (message.SenderUserId != userId && !isTeacher)
                throw new UnauthorizedAccessException("شما دسترسی افزودن فایل به این پیام را ندارید");

            // ثبت فایل روی پیام
            message.AttachmentUrl = request.AttachmentUrl.Trim();
            message.AttachmentName = request.AttachmentName?.Trim();
            message.AttachmentSize = request.AttachmentSize;
            message.AttachmentType = type;

            await _context.SaveChangesAsync();

            return await BuildDtoAsync(message.ID);
        }

    }
}
