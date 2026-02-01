using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class GroupChatReadState : BaseEntity
    {
        public long GroupId { get; set; }
        public Group Group { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public long? LastReadMessageId { get; set; }
        public DateTime? LastReadAt { get; set; }
    }

    public class GroupChatReadStateDto
    {
        public long GroupId { get; set; }
        public long UserId { get; set; }
        public long? LastReadMessageId { get; set; }
        public DateTime? LastReadAt { get; set; }
    }

    public class GroupMemberReadStateDto
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public long? LastReadMessageId { get; set; }
        public DateTime? LastReadAt { get; set; }
    }


}
