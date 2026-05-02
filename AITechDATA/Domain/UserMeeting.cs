using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{ 
    public class UserMeeting : BaseEntity
    {
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public long MeetingId { get; set; }

        [ForeignKey("MeetingId")]
        public Meeting Meeting { get; set; }

    }
}