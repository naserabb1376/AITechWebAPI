using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.Domain
{
    // Duty: جدول وظیفه‌ها
    public class Duty : BaseEntity
    {
        public string DutyTitle { get; set; }
        public string DutyDescription { get; set; }
        public long UserId { get; set; } // کلید خارجی به User
        public User User { get; set; } // ارتباط با User
        public long? SenderUserId { get; set; } // کلید خارجی به User

        [ForeignKey("SenderUserId")]
        public User? SenderUser { get; set; } // ارتباط با User
        public int DutyPassLevel { get; set; }
        public bool IsRead { get; set; }
        public bool IsDone { get; set; }
        public string? DutyReport { get; set; }
        public float DutyScore { get; set; }
    }

    public class EmployeePerformanceChartDto
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public int TotalDuties { get; set; }
        public int DoneCount { get; set; }
        public int NotDoneCount { get; set; }
        public int ReadCount { get; set; }
        public int NotReadCount { get; set; }
        public float AverageScore { get; set; }
        public float DonePercentage { get; set; }
    }

    public class DutyTrendChartDto
    {
        public string Date { get; set; }
        public int TotalCount { get; set; }
        public int DoneCount { get; set; }
    }
}