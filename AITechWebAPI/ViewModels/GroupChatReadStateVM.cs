using AITechDATA.Domain;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechWebAPI.ViewModels
{
    public class GroupChatReadStateVM : BaseVM
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; } // نام گروه
        public string GroupType { get; set; } // نوع گروه
        public string CourseTitle { get; set; } // نام درس
        public string TeacherName { get; set; } // نام مدرس

        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NationalCode { get; set; }
        public string Username { get; set; }
        public DateTime? LastReadAt { get; set; }

        // (اختیاری - برای ریپلای)
        public long? LastReadMessageId { get; set; }
    }

}
