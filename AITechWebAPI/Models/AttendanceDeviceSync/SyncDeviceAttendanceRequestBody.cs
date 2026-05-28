using System.ComponentModel.DataAnnotations;

namespace AITechWebAPI.Models.AttendanceDeviceSync
{
    public class SyncDeviceAttendanceRequestBody
    {
        [Display(Name = "سریال دستگاه")]
        public string? DeviceSerial { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public List<DeviceAttendanceLogRequestBody> Logs { get; set; } = new();
    }

    public class DeviceAttendanceLogRequestBody
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "شناسه کاربر در دستگاه")]
        public string EnrollNumber { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "زمان ثبت دستگاه")]
        public DateTime Timestamp { get; set; }

        public int VerifyMode { get; set; }

        public int InOutMode { get; set; }

        public int WorkCode { get; set; }
    }

    public class SyncDeviceAttendanceResultBody
    {
        public int ReceivedCount { get; set; }
        public int MatchedLogCount { get; set; }
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedSinglePunchDays { get; set; }
        public List<string> UnmatchedDeviceUserIds { get; set; } = new();
    }
}
