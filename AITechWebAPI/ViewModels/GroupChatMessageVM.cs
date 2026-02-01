using AITechDATA.Domain;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class GroupChatMessageVM : BaseVM
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; } // نام گروه
        public string GroupType { get; set; } // نوع گروه
        public string CourseTitle { get; set; } // نام درس
        public string TeacherName { get; set; } // نام مدرس

        public long SenderUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NationalCode { get; set; }
        public string Username { get; set; }

        public string Text { get; set; }              // متن پیام
        public DateTime SentAt { get; set; }         // زمان ارسال (UTC پیشنهاد می‌شود)

        // Edit / Delete
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // (اختیاری - برای ریپلای)
        public long? ReplyToMessageId { get; set; }
        public string RefMessageText { get; set; }   // متن پیام مرجع

        // ✅ Attachment
        public string? AttachmentUrl { get; set; }
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public string? AttachmentType { get; set; } // "images" | "files"
    }

}
