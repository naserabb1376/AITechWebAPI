using AITechDATA.DataLayer;
using AITechDATA.ResultObjects;
using AITechWebAPI.Models.ParentDashboard;
using AITechWebAPI.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AITechWebAPI.Controllers
{
    [Route("ParentDashboard")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ParentDashboardController : ControllerBase
    {
        private readonly AITechContext _db;
        private readonly SchoolAITechContext _schoolDb;

        public ParentDashboardController(AITechContext db, SchoolAITechContext schoolDb)
        {
            _db = db;
            _schoolDb = schoolDb;
        }

        [HttpPost("GetStudentDashboard")]
        public async Task<ActionResult<RowResultObject<ParentStudentDashboardVM>>> GetStudentDashboard(GetParentStudentDashboardRequestBody requestBody)
        {
            var result = new RowResultObject<ParentStudentDashboardVM>();

            if (!ModelState.IsValid)
            {
                result.Status = false;
                result.ErrorMessage = "درخواست نامعتبر است";
                return BadRequest(result);
            }

            if (requestBody.StudentDetailsId <= 0)
            {
                result.Status = false;
                result.ErrorMessage = "کد دانش‌آموز نامعتبر است";
                return BadRequest(result);
            }

            if (IsParentLogin())
            {
                var selectedStudentDetailsId = GetClaimLong("SelectedStudentDetailsId");
                var parentId = GetClaimLong("ParentId");
                var parentPhone = User.FindFirstValue("ParentPhone") ?? "";

                if (selectedStudentDetailsId != requestBody.StudentDetailsId || parentId <= 0 || string.IsNullOrWhiteSpace(parentPhone))
                {
                    result.Status = false;
                    result.ErrorMessage = "دسترسی به اطلاعات این دانش‌آموز مجاز نیست";
                    return Forbid();
                }

                var hasParentAccess = await _db.Parents
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.ID == parentId &&
                        x.ContactNumber == parentPhone &&
                        x.StudentDetailsId == requestBody.StudentDetailsId);

                if (!hasParentAccess)
                {
                    result.Status = false;
                    result.ErrorMessage = "دسترسی والد به این دانش‌آموز تایید نشد";
                    return Forbid();
                }
            }

            var tokenStudentDetailsId = GetCurrentStudentDetailsId();
            if (tokenStudentDetailsId > 0 && tokenStudentDetailsId != requestBody.StudentDetailsId)
            {
                result.Status = false;
                result.ErrorMessage = "دسترسی به اطلاعات این دانش‌آموز مجاز نیست";
                return Forbid();
            }

            var student = await _db.StudentDetails
                .AsNoTracking()
                .Include(x => x.User).ThenInclude(x => x.EducationalBackground)
                .Include(x => x.Parents)
                .FirstOrDefaultAsync(x => x.ID == requestBody.StudentDetailsId);

            if (student?.User == null)
            {
                result.Status = false;
                result.ErrorMessage = "دانش‌آموز یافت نشد";
                return BadRequest(result);
            }

            var currentUserId = User.GetCurrentUserId();
            if (tokenStudentDetailsId <= 0 && currentUserId > 0 && currentUserId != student.UserId)
            {
                result.Status = false;
                result.ErrorMessage = "دسترسی به اطلاعات این دانش‌آموز مجاز نیست";
                return Forbid();
            }

            var dashboard = new ParentStudentDashboardVM
            {
                Student = BuildStudentVm(student)
            };

            var studentUserId = student.UserId;

            dashboard.Classes = await LoadClassesAsync(studentUserId);
            var groupIds = dashboard.Classes.Select(x => x.GroupId).Distinct().ToList();
            dashboard.RecentSessions = await LoadRecentSessionsAsync(studentUserId, groupIds);
            dashboard.Attendance = await LoadAttendanceAsync(studentUserId);
            dashboard.Assignments = await LoadAssignmentsAsync(studentUserId, groupIds);
            dashboard.Grades = await LoadGradesAsync(studentUserId, dashboard.Classes, dashboard.Assignments);
            dashboard.GradeOverview = BuildGradeOverview(dashboard.Grades);
            dashboard.ExamResult = await LoadExamResultAsync(student.ID, student.UserId);
            dashboard.Notifications = await LoadNotificationsAsync(studentUserId);
            dashboard.Summary = BuildSummary(dashboard);
            dashboard.Charts = BuildCharts(dashboard);

            result.Result = dashboard;
            return Ok(result);
        }

        private long GetCurrentStudentDetailsId()
        {
            var idStr = User.FindFirstValue("StudentId");
            return long.TryParse(idStr, out var id) ? id : 0;
        }

        private bool IsParentLogin()
        {
            return string.Equals(User.FindFirstValue("LoginType"), "Parent", StringComparison.OrdinalIgnoreCase);
        }

        private long GetClaimLong(string claimType)
        {
            var idStr = User.FindFirstValue(claimType);
            return long.TryParse(idStr, out var id) ? id : 0;
        }

        private static ParentDashboardStudentVM BuildStudentVm(AITechDATA.Domain.StudentDetails student)
        {
            var user = student.User;
            return new ParentDashboardStudentVM
            {
                StudentDetailsId = student.ID,
                StudentUserId = student.UserId,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                IdentificationCode = user.IdentificationCode,
                NationalCode = user.NationalCode,
                Username = user.Username,
                Email = user.Email,
                EducationalGrade = user.EducationalBackground?.EducationalGrade,
                StudyField = user.EducationalBackground?.StudyField,
                Parents = student.Parents.Select(parent => new ParentDashboardParentVM
                {
                    ParentId = parent.ID,
                    Name = parent.Name,
                    ContactNumber = parent.ContactNumber,
                    Job = parent.Job,
                    Education = parent.Education
                }).ToList()
            };
        }

        private async Task<List<ParentDashboardClassVM>> LoadClassesAsync(long studentUserId)
        {
            var rows = await _db.UserGroups
                .AsNoTracking()
                .Where(x => x.UserId == studentUserId && x.IsActive)
                .Include(x => x.Group).ThenInclude(x => x.Course)
                .Include(x => x.Group).ThenInclude(x => x.Teacher)
                .OrderByDescending(x => x.Group.StartDate)
                .ToListAsync();

            return rows
                .Where(x => x.Group != null)
                .Select(x => new ParentDashboardClassVM
                {
                    GroupId = x.GroupId,
                    GroupName = x.Group.Name,
                    CourseId = x.Group.CourseId,
                    CourseTitle = x.Group.Course?.Title ?? "",
                    TeacherId = x.Group.TeacherId,
                    TeacherName = x.Group.Teacher == null ? "" : $"{x.Group.Teacher.FirstName} {x.Group.Teacher.LastName}".Trim(),
                    DayOfWeek = x.Group.DayOfWeek,
                    StartDate = x.Group.StartDate,
                    EndDate = x.Group.EndDate,
                    StartTime = x.Group.StartTime,
                    EndTime = x.Group.EndTime,
                    GroupType = x.Group.GroupType,
                    Status = x.Group.Status.ToString(),
                    Fee = x.Group.Fee
                })
                .ToList();
        }

        private async Task<List<ParentDashboardSessionVM>> LoadRecentSessionsAsync(long studentUserId, List<long> groupIds)
        {
            if (!groupIds.Any()) return new List<ParentDashboardSessionVM>();

            var rows = await _db.Sessions
                .AsNoTracking()
                .Where(x => groupIds.Contains(x.GroupId))
                .Include(x => x.Group).ThenInclude(x => x.Course)
                .Include(x => x.Attendances.Where(a => a.UserId == studentUserId))
                .Include(x => x.SessionAssignments)
                .OrderByDescending(x => x.SessionDate)
                .Take(100)
                .ToListAsync();

            return rows.Select(session =>
            {
                var attendance = session.Attendances.FirstOrDefault();
                return new ParentDashboardSessionVM
                {
                    SessionId = session.ID,
                    GroupId = session.GroupId,
                    GroupName = session.Group?.Name,
                    CourseTitle = session.Group?.Course?.Title,
                    SessionDate = session.SessionDate,
                    Description = session.Description,
                    Note = session.Note,
                    VideoDurationSeconds = session.VideoDurationSeconds,
                    IsPresent = attendance?.IsPresent,
                    AssignmentCount = session.SessionAssignments?.Count ?? 0
                };
            }).ToList();
        }

        private async Task<ParentDashboardAttendanceVM> LoadAttendanceAsync(long studentUserId)
        {
            var rows = await _db.Attendances
                .AsNoTracking()
                .Where(x => x.UserId == studentUserId)
                .Include(x => x.Session).ThenInclude(x => x.Group).ThenInclude(x => x.Course)
                .OrderByDescending(x => x.Session.SessionDate)
                .ToListAsync();

            var presentCount = rows.Count(x => x.IsPresent);
            var absentCount = rows.Count - presentCount;

            return new ParentDashboardAttendanceVM
            {
                TotalSessions = rows.Count,
                PresentCount = presentCount,
                AbsentCount = absentCount,
                PresentPercent = Percent(presentCount, rows.Count),
                ByGroup = rows
                    .Where(x => x.Session?.Group != null)
                    .GroupBy(x => new
                    {
                        GroupId = x.Session.GroupId,
                        GroupName = x.Session.Group.Name,
                        CourseTitle = x.Session.Group.Course != null ? x.Session.Group.Course.Title : ""
                    })
                    .Select(group =>
                    {
                        var groupPresentCount = group.Count(x => x.IsPresent);
                        var groupTotalCount = group.Count();
                        return new ParentDashboardAttendanceGroupSummaryVM
                        {
                            GroupId = group.Key.GroupId,
                            GroupName = group.Key.GroupName,
                            CourseTitle = group.Key.CourseTitle,
                            TotalSessions = groupTotalCount,
                            PresentCount = groupPresentCount,
                            AbsentCount = groupTotalCount - groupPresentCount,
                            PresentPercent = Percent(groupPresentCount, groupTotalCount)
                        };
                    })
                    .OrderByDescending(x => x.TotalSessions)
                    .ToList(),
                Items = rows.Select(x => new ParentDashboardAttendanceItemVM
                {
                    AttendanceId = x.ID,
                    SessionId = x.SessionId,
                    GroupId = x.Session?.GroupId ?? 0,
                    GroupName = x.Session?.Group?.Name,
                    CourseTitle = x.Session?.Group?.Course?.Title,
                    SessionDate = x.Session?.SessionDate ?? DateTime.MinValue,
                    SessionDescription = x.Session?.Description,
                    IsPresent = x.IsPresent
                }).ToList()
            };
        }

        private async Task<List<ParentDashboardAssignmentVM>> LoadAssignmentsAsync(long studentUserId, List<long> groupIds)
        {
            if (!groupIds.Any()) return new List<ParentDashboardAssignmentVM>();

            var sessionAssignments = await _db.SessionAssignments
                .AsNoTracking()
                .Where(x => groupIds.Contains(x.Session.GroupId))
                .Include(x => x.Session).ThenInclude(x => x.Group).ThenInclude(x => x.Course)
                .OrderByDescending(x => x.DueDate)
                .Take(40)
                .ToListAsync();

            var sessionAssignmentIds = sessionAssignments.Select(x => x.ID).ToList();
            var submittedAssignments = await _db.Assignments
                .AsNoTracking()
                .Where(x => x.UserId == studentUserId && sessionAssignmentIds.Contains(x.SessionAssignmentId))
                .ToListAsync();

            var submittedBySessionAssignment = submittedAssignments
                .GroupBy(x => x.SessionAssignmentId)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(a => a.SubmissionDate).First());

            var assignmentIds = submittedAssignments.Select(x => x.ID).Distinct().ToList();
            var assignmentGrades = assignmentIds.Any()
                ? await _db.ClassGrades
                    .AsNoTracking()
                    .Where(x => x.EntityName.ToLower() == "assignment" && assignmentIds.Contains(x.ForeignKeyId))
                    .ToListAsync()
                : new List<AITechDATA.Domain.ClassGrade>();

            var gradeByAssignmentId = assignmentGrades
                .GroupBy(x => x.ForeignKeyId)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(g => g.CreateDate).First().GradeScore);

            return sessionAssignments.Select(item =>
            {
                submittedBySessionAssignment.TryGetValue(item.ID, out var assignment);
                float? grade = null;
                if (assignment != null && gradeByAssignmentId.TryGetValue(assignment.ID, out var gradeScore))
                {
                    grade = gradeScore;
                }

                return new ParentDashboardAssignmentVM
                {
                    SessionAssignmentId = item.ID,
                    AssignmentId = assignment?.ID,
                    SessionId = item.SessionId,
                    GroupId = item.Session?.GroupId ?? 0,
                    GroupName = item.Session?.Group?.Name,
                    CourseTitle = item.Session?.Group?.Course?.Title,
                    Title = item.Title,
                    Description = item.Description,
                    DueDate = item.DueDate,
                    IsSubmitted = assignment != null,
                    SubmissionDate = assignment?.SubmissionDate,
                    GradeScore = grade
                };
            }).ToList();
        }

        private async Task<List<ParentDashboardGradeVM>> LoadGradesAsync(long studentUserId, List<ParentDashboardClassVM> classes, List<ParentDashboardAssignmentVM> assignments)
        {
            var groupIds = classes.Select(x => x.GroupId).Distinct().ToList();
            var courseIds = classes.Select(x => x.CourseId).Distinct().ToList();
            var assignmentIds = assignments
                .Where(x => x.AssignmentId.HasValue)
                .Select(x => x.AssignmentId!.Value)
                .Distinct()
                .ToList();

            var sessionRows = groupIds.Any()
                ? await _db.Sessions
                    .AsNoTracking()
                    .Where(x => groupIds.Contains(x.GroupId))
                    .Include(x => x.Group).ThenInclude(x => x.Course)
                    .ToListAsync()
                : new List<AITechDATA.Domain.Session>();

            var sessionIds = sessionRows.Select(x => x.ID).Distinct().ToList();

            var query = _db.ClassGrades.AsNoTracking().Where(x =>
                (x.EntityName.ToLower() == "user" && x.ForeignKeyId == studentUserId) ||
                (x.EntityName.ToLower() == "assignment" && assignmentIds.Contains(x.ForeignKeyId)) ||
                (x.EntityName.ToLower() == "session" && sessionIds.Contains(x.ForeignKeyId)) ||
                (x.EntityName.ToLower() == "group" && groupIds.Contains(x.ForeignKeyId)) ||
                (x.EntityName.ToLower() == "course" && courseIds.Contains(x.ForeignKeyId)));

            var rows = await query
                .OrderByDescending(x => x.CreateDate)
                .Take(200)
                .ToListAsync();

            var classByGroupId = classes
                .GroupBy(x => x.GroupId)
                .ToDictionary(x => x.Key, x => x.First());
            var classByCourseId = classes
                .GroupBy(x => x.CourseId)
                .ToDictionary(x => x.Key, x => x.First());
            var assignmentById = assignments
                .Where(x => x.AssignmentId.HasValue)
                .GroupBy(x => x.AssignmentId!.Value)
                .ToDictionary(x => x.Key, x => x.First());
            var sessionById = sessionRows
                .GroupBy(x => x.ID)
                .ToDictionary(x => x.Key, x => x.First());

            return rows.Select(x =>
            {
                var grade = new ParentDashboardGradeVM
                {
                    GradeId = x.ID,
                    EntityName = x.EntityName,
                    ForeignKeyId = x.ForeignKeyId,
                    SourceLabel = ResolveGradeSourceLabel(x.EntityName),
                    Title = x.Title,
                    Description = x.Description,
                    GradeScore = x.GradeScore,
                    CreateDate = x.CreateDate
                };

                var entityName = x.EntityName.ToLower();
                if (entityName == "assignment" && assignmentById.TryGetValue(x.ForeignKeyId, out var assignment))
                {
                    grade.GroupId = assignment.GroupId;
                    grade.GroupName = assignment.GroupName;
                    grade.CourseTitle = assignment.CourseTitle;
                    grade.SessionId = assignment.SessionId;
                    grade.SourceLabel = "تکلیف";
                }
                else if (entityName == "session" && sessionById.TryGetValue(x.ForeignKeyId, out var session))
                {
                    grade.GroupId = session.GroupId;
                    grade.GroupName = session.Group?.Name;
                    grade.CourseTitle = session.Group?.Course?.Title;
                    grade.SessionId = session.ID;
                    grade.SessionDate = session.SessionDate;
                    grade.SourceLabel = "جلسه";
                }
                else if (entityName == "group" && classByGroupId.TryGetValue(x.ForeignKeyId, out var groupClass))
                {
                    grade.GroupId = groupClass.GroupId;
                    grade.GroupName = groupClass.GroupName;
                    grade.CourseTitle = groupClass.CourseTitle;
                    grade.SourceLabel = "کلاس";
                }
                else if (entityName == "course" && classByCourseId.TryGetValue(x.ForeignKeyId, out var courseClass))
                {
                    grade.GroupId = courseClass.GroupId;
                    grade.GroupName = courseClass.GroupName;
                    grade.CourseTitle = courseClass.CourseTitle;
                    grade.SourceLabel = "دوره";
                }
                else if (entityName == "user")
                {
                    grade.SourceLabel = "دانش‌آموز";
                }

                return grade;
            }).ToList();
        }

        private static ParentDashboardGradeOverviewVM BuildGradeOverview(List<ParentDashboardGradeVM> grades)
        {
            return new ParentDashboardGradeOverviewVM
            {
                ByGroup = grades
                    .Where(x => x.GroupId.HasValue)
                    .GroupBy(x => new { x.GroupId, Label = string.IsNullOrWhiteSpace(x.GroupName) ? x.CourseTitle ?? "کلاس" : x.GroupName })
                    .Select(x => BuildGradeSummary($"group-{x.Key.GroupId}", x.Key.Label, x, x.Key.GroupId, null))
                    .OrderByDescending(x => x.AverageGrade)
                    .ToList(),
                BySession = grades
                    .Where(x => x.SessionId.HasValue)
                    .GroupBy(x => new
                    {
                        x.SessionId,
                        Label = x.SessionDate.HasValue
                            ? $"{(string.IsNullOrWhiteSpace(x.GroupName) ? "جلسه" : x.GroupName)} - {x.SessionDate.Value:yyyy/MM/dd}"
                            : string.IsNullOrWhiteSpace(x.GroupName) ? "جلسه" : x.GroupName
                    })
                    .Select(x => BuildGradeSummary($"session-{x.Key.SessionId}", x.Key.Label, x, null, x.Key.SessionId))
                    .OrderByDescending(x => x.AverageGrade)
                    .ToList(),
                ByEntity = grades
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.SourceLabel) ? x.EntityName : x.SourceLabel)
                    .Select(x => BuildGradeSummary(x.Key, x.Key, x, null, null))
                    .OrderByDescending(x => x.AverageGrade)
                    .ToList()
            };
        }

        private static ParentDashboardGradeSummaryVM BuildGradeSummary(
            string key,
            string label,
            IEnumerable<ParentDashboardGradeVM> grades,
            long? groupId,
            long? sessionId)
        {
            var rows = grades.ToList();
            return new ParentDashboardGradeSummaryVM
            {
                Key = key,
                Label = label,
                GroupId = groupId,
                SessionId = sessionId,
                GradeCount = rows.Count,
                AverageGrade = rows.Any() ? Math.Round((decimal)rows.Average(x => x.GradeScore), 2) : 0,
                MinGrade = rows.Any() ? rows.Min(x => x.GradeScore) : 0,
                MaxGrade = rows.Any() ? rows.Max(x => x.GradeScore) : 0
            };
        }

        private static string ResolveGradeSourceLabel(string? entityName)
        {
            return entityName?.ToLower() switch
            {
                "user" => "دانش‌آموز",
                "assignment" => "تکلیف",
                "session" => "جلسه",
                "group" => "کلاس",
                "course" => "دوره",
                _ => entityName ?? ""
            };
        }

        private async Task<ParentDashboardExamResultVM?> LoadExamResultAsync(long studentDetailsId, long studentUserId)
        {
            var registration = await _schoolDb.SchoolRegistrations
                .AsNoTracking()
                .Where(x => x.StudentDetailsId == studentDetailsId || x.UserId == studentUserId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefaultAsync();

            if (registration == null) return null;

            var examResult = await _schoolDb.SchoolExamResults
                .AsNoTracking()
                .Where(x => x.SchoolRegistrationId == registration.ID)
                .OrderByDescending(x => x.UpdateDate)
                .ThenByDescending(x => x.ID)
                .FirstOrDefaultAsync();

            if (examResult == null)
            {
                return new ParentDashboardExamResultVM
                {
                    SchoolRegistrationId = registration.ID,
                    CurrentSchoolName = registration.CurrentSchoolName,
                    TargetGrade = registration.TargetGrade,
                    RegistrationStatus = registration.RegistrationStatus,
                    SeatNumber = registration.SeatNumber
                };
            }

            return new ParentDashboardExamResultVM
            {
                SchoolRegistrationId = registration.ID,
                ExamResultId = examResult.ID,
                Grade = examResult.Grade,
                CurrentSchoolName = registration.CurrentSchoolName,
                TargetGrade = registration.TargetGrade,
                RegistrationStatus = registration.RegistrationStatus,
                SeatNumber = registration.SeatNumber,
                TotalQuestions = examResult.TotalQuestions,
                CorrectCount = examResult.CorrectCount,
                WrongCount = examResult.WrongCount,
                BlankCount = examResult.BlankCount,
                ScorePercent = examResult.ScorePercent,
                IsAccepted = examResult.IsAccepted,
                AcceptanceMessage = examResult.AcceptanceMessage,
                IsFinalAccepted = examResult.IsFinalAccepted,
                FinalAcceptanceStatus = examResult.FinalAcceptanceStatus,
                Published = examResult.Published,
                SectionScoresJson = examResult.SectionScoresJson
            };
        }

        private async Task<List<ParentDashboardNotificationVM>> LoadNotificationsAsync(long studentUserId)
        {
            var rows = await _db.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == studentUserId)
                .Include(x => x.SenderUser)
                .OrderByDescending(x => x.CreateDate)
                .Take(10)
                .ToListAsync();

            return rows.Select(x => new ParentDashboardNotificationVM
            {
                NotificationId = x.ID,
                Message = x.Message,
                SenderName = x.SenderUser == null ? null : $"{x.SenderUser.FirstName} {x.SenderUser.LastName}".Trim(),
                IsRead = x.IsRead,
                NotificationResponse = x.NotificationResponse,
                CreateDate = x.CreateDate
            }).ToList();
        }

        private static ParentDashboardSummaryVM BuildSummary(ParentStudentDashboardVM dashboard)
        {
            var activeClassCount = dashboard.Classes.Count(x => string.Equals(x.Status, "Active", StringComparison.OrdinalIgnoreCase));
            var submittedAssignmentCount = dashboard.Assignments.Count(x => x.IsSubmitted);
            var pendingAssignmentCount = dashboard.Assignments.Count(x => !x.IsSubmitted);

            return new ParentDashboardSummaryVM
            {
                ActiveClassCount = activeClassCount,
                TotalClassCount = dashboard.Classes.Count,
                TotalSessionCount = dashboard.Attendance.TotalSessions,
                PresentCount = dashboard.Attendance.PresentCount,
                AbsentCount = dashboard.Attendance.AbsentCount,
                PresentPercent = dashboard.Attendance.PresentPercent,
                SubmittedAssignmentCount = submittedAssignmentCount,
                PendingAssignmentCount = pendingAssignmentCount,
                AverageGrade = dashboard.Grades.Any() ? Math.Round((decimal)dashboard.Grades.Average(x => x.GradeScore), 2) : 0,
                UnreadNotificationCount = dashboard.Notifications.Count(x => !x.IsRead)
            };
        }

        private static ParentDashboardChartsVM BuildCharts(ParentStudentDashboardVM dashboard)
        {
            var charts = new ParentDashboardChartsVM
            {
                AttendanceDonut = new List<ParentDashboardChartItemVM>
                {
                    new() { Label = "حاضر", Value = dashboard.Attendance.PresentCount },
                    new() { Label = "غایب", Value = dashboard.Attendance.AbsentCount }
                },
                AssignmentStatus = new List<ParentDashboardChartItemVM>
                {
                    new() { Label = "ارسال شده", Value = dashboard.Assignments.Count(x => x.IsSubmitted) },
                    new() { Label = "ارسال نشده", Value = dashboard.Assignments.Count(x => !x.IsSubmitted) }
                },
                GradeTrend = dashboard.Grades
                    .OrderBy(x => x.CreateDate)
                    .Select(x => new ParentDashboardGradeTrendItemVM
                    {
                        Label = x.Title ?? x.EntityName,
                        Date = x.CreateDate,
                        Value = x.GradeScore
                    })
                    .ToList()
            };

            if (dashboard.ExamResult != null && dashboard.ExamResult.ExamResultId > 0)
            {
                charts.ExamAnswerStatus = new List<ParentDashboardChartItemVM>
                {
                    new() { Label = "صحیح", Value = dashboard.ExamResult.CorrectCount },
                    new() { Label = "غلط", Value = dashboard.ExamResult.WrongCount },
                    new() { Label = "نزده", Value = dashboard.ExamResult.BlankCount }
                };
            }

            return charts;
        }

        private static decimal Percent(int value, int total)
        {
            return total <= 0 ? 0 : Math.Round((decimal)value * 100 / total, 2);
        }
    }
}
