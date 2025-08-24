using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    public class ClassForAi
    {
        [Key]
        public int ID { get; set; }

        public string CourseName { get; set; }

        public string GroupName { get; set; }

        public string TeacherName { get; set; }

        public long? Price { get; set; }

        public string PreCourse { get; set; }

        public int? NumberOfUser { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string GroupType { get; set; }

        public string AgeLimit { get; set; }
    }
}
