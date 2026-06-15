namespace AITechWebAPI.Models.ParentDashboard
{
    public class GetParentStudentDashboardRequestBody
    {
        public long StudentDetailsId { get; set; }
    }

    public class ParentStudentDashboardVM
    {
        public ParentDashboardStudentVM Student { get; set; } = new();
        public ParentDashboardSummaryVM Summary { get; set; } = new();
        public ParentDashboardAttendanceVM Attendance { get; set; } = new();
        public List<ParentDashboardClassVM> Classes { get; set; } = new();
        public List<ParentDashboardSessionVM> RecentSessions { get; set; } = new();
        public List<ParentDashboardAssignmentVM> Assignments { get; set; } = new();
        public List<ParentDashboardGradeVM> Grades { get; set; } = new();
        public ParentDashboardGradeOverviewVM GradeOverview { get; set; } = new();
        public ParentDashboardExamResultVM? ExamResult { get; set; }
        public List<ParentDashboardNotificationVM> Notifications { get; set; } = new();
        public ParentDashboardChartsVM Charts { get; set; } = new();
    }

    public class ParentDashboardStudentVM
    {
        public long StudentDetailsId { get; set; }
        public long StudentUserId { get; set; }
        public string FullName { get; set; } = "";
        public string? IdentificationCode { get; set; }
        public string? NationalCode { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? EducationalGrade { get; set; }
        public string? StudyField { get; set; }
        public List<ParentDashboardParentVM> Parents { get; set; } = new();
    }

    public class ParentDashboardParentVM
    {
        public long ParentId { get; set; }
        public string Name { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string? Job { get; set; }
        public string? Education { get; set; }
    }

    public class ParentDashboardSummaryVM
    {
        public int ActiveClassCount { get; set; }
        public int TotalClassCount { get; set; }
        public int TotalSessionCount { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal PresentPercent { get; set; }
        public int PendingAssignmentCount { get; set; }
        public int SubmittedAssignmentCount { get; set; }
        public decimal AverageGrade { get; set; }
        public int UnreadNotificationCount { get; set; }
    }

    public class ParentDashboardAttendanceVM
    {
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal PresentPercent { get; set; }
        public List<ParentDashboardAttendanceItemVM> Items { get; set; } = new();
        public List<ParentDashboardAttendanceGroupSummaryVM> ByGroup { get; set; } = new();
    }

    public class ParentDashboardAttendanceItemVM
    {
        public long AttendanceId { get; set; }
        public long SessionId { get; set; }
        public long GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? CourseTitle { get; set; }
        public DateTime SessionDate { get; set; }
        public string? SessionDescription { get; set; }
        public bool IsPresent { get; set; }
    }

    public class ParentDashboardAttendanceGroupSummaryVM
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public string CourseTitle { get; set; } = "";
        public int TotalSessions { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal PresentPercent { get; set; }
    }

    public class ParentDashboardClassVM
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public long CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public long TeacherId { get; set; }
        public string TeacherName { get; set; } = "";
        public string DayOfWeek { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string GroupType { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal Fee { get; set; }
    }

    public class ParentDashboardSessionVM
    {
        public long SessionId { get; set; }
        public long GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? CourseTitle { get; set; }
        public DateTime SessionDate { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public int? VideoDurationSeconds { get; set; }
        public bool? IsPresent { get; set; }
        public int AssignmentCount { get; set; }
    }

    public class ParentDashboardAssignmentVM
    {
        public long SessionAssignmentId { get; set; }
        public long? AssignmentId { get; set; }
        public long SessionId { get; set; }
        public long GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? CourseTitle { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DueDate { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public float? GradeScore { get; set; }
    }

    public class ParentDashboardGradeVM
    {
        public long GradeId { get; set; }
        public string EntityName { get; set; } = "";
        public long ForeignKeyId { get; set; }
        public string SourceLabel { get; set; } = "";
        public long? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? CourseTitle { get; set; }
        public long? SessionId { get; set; }
        public DateTime? SessionDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public float GradeScore { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class ParentDashboardGradeOverviewVM
    {
        public List<ParentDashboardGradeSummaryVM> ByGroup { get; set; } = new();
        public List<ParentDashboardGradeSummaryVM> BySession { get; set; } = new();
        public List<ParentDashboardGradeSummaryVM> ByEntity { get; set; } = new();
    }

    public class ParentDashboardGradeSummaryVM
    {
        public string Key { get; set; } = "";
        public string Label { get; set; } = "";
        public long? GroupId { get; set; }
        public long? SessionId { get; set; }
        public int GradeCount { get; set; }
        public decimal AverageGrade { get; set; }
        public float MinGrade { get; set; }
        public float MaxGrade { get; set; }
    }

    public class ParentDashboardExamResultVM
    {
        public long SchoolRegistrationId { get; set; }
        public long ExamResultId { get; set; }
        public string Grade { get; set; } = "";
        public string CurrentSchoolName { get; set; } = "";
        public string TargetGrade { get; set; } = "";
        public string RegistrationStatus { get; set; } = "";
        public int? SeatNumber { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int BlankCount { get; set; }
        public decimal ScorePercent { get; set; }
        public bool? IsAccepted { get; set; }
        public string AcceptanceMessage { get; set; } = "";
        public bool? IsFinalAccepted { get; set; }
        public string FinalAcceptanceStatus { get; set; } = "";
        public bool Published { get; set; }
        public string SectionScoresJson { get; set; } = "[]";
    }

    public class ParentDashboardNotificationVM
    {
        public long NotificationId { get; set; }
        public string Message { get; set; } = "";
        public string? SenderName { get; set; }
        public bool IsRead { get; set; }
        public string? NotificationResponse { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class ParentDashboardChartsVM
    {
        public List<ParentDashboardChartItemVM> AttendanceDonut { get; set; } = new();
        public List<ParentDashboardChartItemVM> AssignmentStatus { get; set; } = new();
        public List<ParentDashboardGradeTrendItemVM> GradeTrend { get; set; } = new();
        public List<ParentDashboardChartItemVM> ExamAnswerStatus { get; set; } = new();
    }

    public class ParentDashboardChartItemVM
    {
        public string Label { get; set; } = "";
        public decimal Value { get; set; }
    }

    public class ParentDashboardGradeTrendItemVM
    {
        public string Label { get; set; } = "";
        public DateTime? Date { get; set; }
        public float Value { get; set; }
    }
}
