using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class GroupChatMessage : BaseEntity
    {
        public long GroupId { get; set; }
        public Group Group { get; set; }

        public long SenderUserId { get; set; }
        public User SenderUser { get; set; }

        public string Text { get; set; }              // متن پیام
        public DateTime SentAt { get; set; } = DateTime.Now.ToShamsi();        // زمان ارسال (UTC پیشنهاد می‌شود)

        // Edit / Delete
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // (اختیاری - برای ریپلای)
        public long? ReplyToMessageId { get; set; }
        public GroupChatMessage? ReplyToMessage { get; set; }

        // ✅ Attachment
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public string? AttachmentType { get; set; } // "images" | "files"
    }

    public class SendGroupMessageRequest
    {
        public string Text { get; set; }
        public long? ReplyToMessageId { get; set; }
    }

    public class EditGroupMessageRequest
    {
        public string Text { get; set; }
    }


    public class GroupChatMessageDto
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long SenderUserId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
        public long? ReplyToMessageId { get; set; }

        // ✅ Attachment
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public string? AttachmentType { get; set; } // "images" | "files"
    }

    public class AttachFileToMessageRequest
    {
        public string AttachmentUrl { get; set; } = null!;
        public string AttachmentName { get; set; } = null!;
        public long? AttachmentSize { get; set; }
        public string AttachmentType { get; set; } = null!; // images/files
    }

   
}
