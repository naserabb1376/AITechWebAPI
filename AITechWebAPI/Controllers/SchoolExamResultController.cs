using System.Text.Json;
using AITechDATA.DataLayer;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using AITechWebAPI.Models.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITechWebAPI.Controllers
{
    [Route("SchoolExamResult")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class SchoolExamResultController : ControllerBase
    {
        private const string TenantKey = "takinschool";
        private const string Grade4ExamKey = "takinschool-grade4-entrance-2026";
        private const string Grade5ExamKey = "takinschool-grade5-entrance-2026";
        private const string Grade6ExamKey = "takinschool-grade6-entrance-2026";
        private const string FinalStatusAccepted = "accepted";
        private const string FinalStatusRejected = "rejected";
        private const string FinalStatusReserved = "reserved";
        private const string FinalStatusPending = "pending";

        private readonly SchoolAITechContext _schoolDb;

        public SchoolExamResultController(SchoolAITechContext schoolDb)
        {
            _schoolDb = schoolDb;
        }

        [HttpPost("GetAllTakinSchoolExamResults")]
        [AllowAnonymous]
        public async Task<ActionResult<ListResultObject<TakinSchoolExamResultVM>>> GetAllTakinSchoolExamResults(GetListRequestBody requestBody)
        {
            var result = new ListResultObject<TakinSchoolExamResultVM>();
            var pageIndex = Math.Max(requestBody.PageIndex, 1);
            var pageSize = requestBody.PageSize <= 0 ? 10 : requestBody.PageSize;
            var searchText = (requestBody.SearchText ?? "").Trim();

            try
            {
                var query = ResultQuery();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(x =>
                        x.Grade.Contains(searchText) ||
                        x.SchoolRegistration.User.FirstName.Contains(searchText) ||
                        x.SchoolRegistration.User.LastName.Contains(searchText) ||
                        x.SchoolRegistration.User.NationalCode.Contains(searchText) ||
                        x.SchoolRegistration.User.Username.Contains(searchText));
                }

                result.TotalCount = await query.CountAsync();
                result.PageCount = DbTools.GetPageCount(result.TotalCount, pageSize);
                var rows = await query
                    .OrderByDescending(x => x.UpdateDate)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ApplyCurrentScoring(rows);
                result.Results = rows.Select(ToVm).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("GetTakinSchoolExamResultByRegistrationId")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolExamResultVM>>> GetTakinSchoolExamResultByRegistrationId(GetSchoolExamResultByRegistrationRequestBody requestBody)
        {
            var result = new RowResultObject<TakinSchoolExamResultVM>();

            try
            {
                var grade = NormalizeGrade(requestBody.Grade, null);
                var examKey = GetExamKey(grade);
                var row = await ResultQuery()
                    .Where(x => x.SchoolRegistrationId == requestBody.SchoolRegistrationId && x.ExamKey == examKey)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefaultAsync();

                if (row == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "نتیجه آزمون برای این دانش‌آموز ثبت نشده است";
                    return Ok(result);
                }

                ApplyCurrentScoring(row);
                result.Result = ToVm(row);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("SaveTakinSchoolExamResult")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolExamResultVM>>> SaveTakinSchoolExamResult(SaveTakinSchoolExamResultRequestBody requestBody)
        {
            var result = new RowResultObject<TakinSchoolExamResultVM>();

            var registration = await _schoolDb.SchoolRegistrations
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ID == requestBody.SchoolRegistrationId && x.TenantKey == TenantKey);

            if (registration == null)
            {
                result.Status = false;
                result.ErrorMessage = "ثبت‌نام مدرسه پیدا نشد";
                return BadRequest(result);
            }

            var grade = NormalizeGrade(requestBody.Grade, registration.TargetGrade);
            var examKey = GetExamKey(grade);
            var answerKey = GetAnswerKey(grade);
            if (answerKey.Count == 0)
            {
                result.Status = false;
                result.ErrorMessage = "پاسخ‌نامه این پایه هنوز تعریف نشده است";
                return BadRequest(result);
            }

            var answers = NormalizeAnswers(requestBody.Answers, answerKey.Count);
            var score = ScoreAnswers(answers, answerKey, GetSections(grade), grade);
            var now = DateTime.Now.ToShamsi();

            try
            {
                var row = await _schoolDb.SchoolExamResults
                    .FirstOrDefaultAsync(x => x.SchoolRegistrationId == registration.ID && x.ExamKey == examKey);

                if (row == null)
                {
                    row = new SchoolExamResult
                    {
                        CreateDate = now,
                        IsActive = true,
                        SchoolRegistrationId = registration.ID,
                        TenantKey = TenantKey,
                        ExamKey = examKey,
                    };
                    await _schoolDb.SchoolExamResults.AddAsync(row);
                }

                row.UpdateDate = now;
                row.Grade = grade;
                row.TotalQuestions = answerKey.Count;
                row.CorrectCount = score.CorrectCount;
                row.WrongCount = score.WrongCount;
                row.BlankCount = score.BlankCount;
                row.ScorePercent = score.ScorePercent;
                row.IsAccepted = requestBody.IsAccepted;
                row.AcceptanceMessage = requestBody.AcceptanceMessage?.Trim() ?? "";
                row.AnswersJson = JsonSerializer.Serialize(answers);
                row.SectionScoresJson = JsonSerializer.Serialize(score.Sections);
                row.EntrySource = string.IsNullOrWhiteSpace(requestBody.EntrySource) ? "Manual" : requestBody.EntrySource.Trim();
                row.OmrConfidence = requestBody.OmrConfidence;
                row.OmrReviewRequired = requestBody.OmrReviewRequired;
                row.RawOmrJson = requestBody.RawOmrJson;
                row.Published = requestBody.Published;
                row.PublishedAt = requestBody.Published ? (row.PublishedAt ?? now) : null;

                await _schoolDb.SaveChangesAsync();

                var saved = await ResultQuery().FirstAsync(x => x.ID == row.ID);
                result.Result = ToVm(saved);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("GetPublicTakinSchoolExamResult")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<PublicTakinSchoolExamResultVM>>> GetPublicTakinSchoolExamResult(PublicTakinSchoolExamResultRequestBody requestBody)
        {
            var result = new RowResultObject<PublicTakinSchoolExamResultVM>();
            var phoneNumber = OnlyDigits(requestBody.PhoneNumber);
            var nationalCode = OnlyDigits(requestBody.NationalCode);

            try
            {
                var row = await ResultQuery()
                    .Where(x =>
                        x.Published &&
                        x.SchoolRegistration.User.Username == phoneNumber &&
                        x.SchoolRegistration.User.NationalCode == nationalCode)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefaultAsync();

                if (row == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "با احترام، اطلاعاتی برای شما یافت نشد یا کارنامه شما هنوز منتشر نشده است.";
                    return Ok(result);
                }

                var publishedGradeRows = await ResultQuery()
                    .AsNoTracking()
                    .Where(x => x.Published && x.TenantKey == TenantKey && x.ExamKey == row.ExamKey && x.Grade == row.Grade)
                    .ToListAsync();

                ApplyCurrentScoring(publishedGradeRows);
                ApplyCurrentScoring(row);
                var vm = ToPublicVm(row, publishedGradeRows);
                result.Result = vm;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("GetTakinSchoolExamAnalytics")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolExamAnalyticsVM>>> GetTakinSchoolExamAnalytics()
        {
            var result = new RowResultObject<TakinSchoolExamAnalyticsVM>();

            try
            {
                var rows = await ResultQuery()
                    .OrderByDescending(x => x.ScorePercent)
                    .ThenByDescending(x => x.CorrectCount)
                    .ThenBy(x => x.WrongCount)
                    .ToListAsync();

                ApplyCurrentScoring(rows);
                var gradeAnalytics = rows
                    .GroupBy(x => NormalizeGrade(x.Grade, null))
                    .OrderBy(x => GradeSortIndex(x.Key))
                    .Select(group => BuildGradeAnalytics(group.Key, group.ToList()))
                    .ToList();

                result.Result = new TakinSchoolExamAnalyticsVM
                {
                    TotalParticipants = rows.Count,
                    PublishedCount = rows.Count(x => x.Published),
                    AcceptedCount = rows.Count(x => x.IsAccepted == true),
                    PendingAcceptanceCount = rows.Count(x => !x.IsAccepted.HasValue),
                    FinalAcceptedCount = rows.Count(x => GetFinalAcceptanceStatus(x) == FinalStatusAccepted),
                    PendingFinalAcceptanceCount = rows.Count(x => GetFinalAcceptanceStatus(x) == FinalStatusPending),
                    AverageScorePercent = rows.Count > 0 ? Math.Round(rows.Average(x => x.ScorePercent), 2) : 0,
                    HighestScorePercent = rows.Count > 0 ? rows.Max(x => x.ScorePercent) : 0,
                    Grades = gradeAnalytics,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("SetTakinSchoolExamInterviewStatus")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolExamResultVM>>> SetTakinSchoolExamInterviewStatus(SetTakinSchoolExamInterviewStatusRequestBody requestBody)
        {
            var result = new RowResultObject<TakinSchoolExamResultVM>();

            try
            {
                var grade = NormalizeGrade(requestBody.Grade, null);
                var examKey = GetExamKey(grade);
                var row = await _schoolDb.SchoolExamResults
                    .Include(x => x.SchoolRegistration)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x =>
                        x.TenantKey == TenantKey &&
                        x.SchoolRegistrationId == requestBody.SchoolRegistrationId &&
                        x.ExamKey == examKey);

                if (row == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "نتیجه آزمون برای این دانش‌آموز پیدا نشد";
                    return Ok(result);
                }

                row.IsAccepted = requestBody.InvitedToInterview;
                row.AcceptanceMessage = requestBody.InvitedToInterview switch
                {
                    true => string.IsNullOrWhiteSpace(requestBody.Message) ? "دانش‌آموز برای مصاحبه دعوت شده است." : requestBody.Message.Trim(),
                    false => string.IsNullOrWhiteSpace(requestBody.Message) ? "دانش‌آموز برای مصاحبه دعوت نشده است." : requestBody.Message.Trim(),
                    _ => string.IsNullOrWhiteSpace(requestBody.Message) ? "" : requestBody.Message.Trim(),
                };
                row.UpdateDate = DateTime.Now.ToShamsi();

                await _schoolDb.SaveChangesAsync();

                result.Result = ToVm(row);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("SetTakinSchoolExamFinalAcceptanceStatus")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<TakinSchoolExamResultVM>>> SetTakinSchoolExamFinalAcceptanceStatus(SetTakinSchoolExamFinalAcceptanceStatusRequestBody requestBody)
        {
            var result = new RowResultObject<TakinSchoolExamResultVM>();

            try
            {
                var grade = NormalizeGrade(requestBody.Grade, null);
                var examKey = GetExamKey(grade);
                var row = await _schoolDb.SchoolExamResults
                    .Include(x => x.SchoolRegistration)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x =>
                        x.TenantKey == TenantKey &&
                        x.SchoolRegistrationId == requestBody.SchoolRegistrationId &&
                        x.ExamKey == examKey);

                if (row == null)
                {
                    result.Status = false;
                    result.ErrorMessage = "نتیجه آزمون برای این دانش‌آموز پیدا نشد";
                    return Ok(result);
                }

                var finalStatus = NormalizeFinalAcceptanceStatus(requestBody.FinalAcceptanceStatus, requestBody.IsFinalAccepted);
                row.FinalAcceptanceStatus = finalStatus;
                row.IsFinalAccepted = ToLegacyFinalAccepted(finalStatus);
                row.UpdateDate = DateTime.Now.ToShamsi();
                await _schoolDb.SaveChangesAsync();

                result.Result = ToVm(row);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        [HttpPost("BulkUpdateTakinSchoolExamResults")]
        [AllowAnonymous]
        public async Task<ActionResult<RowResultObject<int>>> BulkUpdateTakinSchoolExamResults(BulkUpdateTakinSchoolExamResultsRequestBody requestBody)
        {
            var result = new RowResultObject<int>();
            var registrationIds = requestBody.SchoolRegistrationIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (registrationIds.Count == 0)
            {
                result.Status = false;
                result.ErrorMessage = "حداقل یک دانش‌آموز را انتخاب کنید";
                return BadRequest(result);
            }

            if (!requestBody.UpdateInterviewStatus && !requestBody.UpdateFinalAcceptanceStatus && !requestBody.UpdatePublished)
            {
                result.Status = false;
                result.ErrorMessage = "هیچ تغییری برای اعمال انتخاب نشده است";
                return BadRequest(result);
            }

            try
            {
                var grade = NormalizeGrade(requestBody.Grade, null);
                var examKey = GetExamKey(grade);
                var rows = await _schoolDb.SchoolExamResults
                    .Where(x =>
                        x.TenantKey == TenantKey &&
                        x.ExamKey == examKey &&
                        registrationIds.Contains(x.SchoolRegistrationId))
                    .ToListAsync();

                var now = DateTime.Now.ToShamsi();
                foreach (var row in rows)
                {
                    if (requestBody.UpdateInterviewStatus)
                    {
                        row.IsAccepted = requestBody.InvitedToInterview;
                        row.AcceptanceMessage = requestBody.InvitedToInterview switch
                        {
                            true => "دانش‌آموز برای مصاحبه دعوت شده است.",
                            false => "دانش‌آموز برای مصاحبه دعوت نشده است.",
                            _ => "",
                        };
                    }

                    if (requestBody.UpdatePublished)
                    {
                        row.Published = requestBody.Published;
                        row.PublishedAt = requestBody.Published ? (row.PublishedAt ?? now) : null;
                    }

                    if (requestBody.UpdateFinalAcceptanceStatus)
                    {
                        var finalStatus = NormalizeFinalAcceptanceStatus(requestBody.FinalAcceptanceStatus, requestBody.IsFinalAccepted);
                        row.FinalAcceptanceStatus = finalStatus;
                        row.IsFinalAccepted = ToLegacyFinalAccepted(finalStatus);
                    }

                    row.UpdateDate = now;
                }

                await _schoolDb.SaveChangesAsync();
                result.Result = rows.Count;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
                return BadRequest(result);
            }
        }

        private IQueryable<SchoolExamResult> ResultQuery()
        {
            return _schoolDb.SchoolExamResults
                .AsNoTracking()
                .Where(x => x.TenantKey == TenantKey)
                .Include(x => x.SchoolRegistration)
                    .ThenInclude(x => x.User);
        }

        private static Dictionary<int, int> GetAnswerKey(string grade)
        {
            if (grade == "پنجم")
            {
                return new Dictionary<int, int>
                {
                    [1] = 2, [2] = 3, [3] = 4, [4] = 1, [5] = 3,
                    [6] = 2, [7] = 4, [8] = 2, [9] = 1, [10] = 2,
                    [11] = 2, [12] = 2, [13] = 1, [14] = 3, [15] = 4,
                    [16] = 1, [17] = 2, [18] = 4, [19] = 3, [20] = 2,
                    [21] = 2, [22] = 3, [23] = 3, [24] = 4, [25] = 4,
                    [26] = 4, [27] = 4, [28] = 2, [29] = 4, [30] = 3,
                    [31] = 4, [32] = 4, [33] = 4, [34] = 2, [35] = 3,
                    [36] = 2, [37] = 3, [38] = 4, [39] = 4, [40] = 4,
                    [41] = 3, [42] = 1, [43] = 2, [44] = 4, [45] = 1,
                    [46] = 3, [47] = 2, [48] = 3, [49] = 1, [50] = 2,
                    [51] = 1, [52] = 2, [53] = 3, [54] = 1, [55] = 2,
                    [56] = 2, [57] = 3, [58] = 1, [59] = 2, [60] = 3
                };
            }

            if (grade == "ششم")
            {
                return new Dictionary<int, int>
                {
                    [1] = 3, [2] = 2, [3] = 1, [4] = 2, [5] = 4,
                    [6] = 1, [7] = 4, [8] = 2, [9] = 1, [10] = 3,
                    [11] = 3, [12] = 2, [13] = 2, [14] = 4, [15] = 4,
                    [16] = 2, [17] = 3, [18] = 4, [19] = 2, [20] = 2,
                    [21] = 2, [22] = 3, [23] = 3, [24] = 1, [25] = 3,
                    [26] = 4, [27] = 2, [28] = 4, [29] = 2, [30] = 3,
                    [31] = 3, [32] = 2, [33] = 2, [34] = 3, [35] = 3,
                    [36] = 3, [37] = 4, [38] = 4, [39] = 4, [40] = 3,
                    [41] = 2, [42] = 3, [43] = 1, [44] = 2, [45] = 3,
                    [46] = 4, [47] = 2, [48] = 1, [49] = 1, [50] = 2,
                    [51] = 3, [52] = 2, [53] = 4, [54] = 1, [55] = 2,
                    [56] = 1, [57] = 2, [58] = 4, [59] = 1, [60] = 2
                };
            }

            if (grade != "چهارم")
            {
                return new Dictionary<int, int>();
            }

            return new Dictionary<int, int>
            {
                [1] = 3, [2] = 2, [3] = 4, [4] = 1, [5] = 4,
                [6] = 3, [7] = 1, [8] = 2, [9] = 2, [10] = 1,
                [11] = 1, [12] = 2, [13] = 4, [14] = 1, [15] = 2,
                [16] = 2, [17] = 4, [18] = 4, [19] = 4, [20] = 2,
                [21] = 4, [22] = 1, [23] = 4, [24] = 2, [25] = 1,
                [26] = 1, [27] = 2, [28] = 3, [29] = 3, [30] = 3,
                [31] = 2, [32] = 3, [33] = 1, [34] = 2, [35] = 3,
                [36] = 2, [37] = 3, [38] = 2, [39] = 3, [40] = 1,
                [41] = 1, [42] = 2, [43] = 3, [44] = 4, [45] = 2,
                [46] = 3, [47] = 1, [48] = 2, [49] = 3, [50] = 2
            };
        }

        private static string GetExamKey(string grade)
        {
            return grade switch
            {
                "پنجم" => Grade5ExamKey,
                "ششم" => Grade6ExamKey,
                _ => Grade4ExamKey,
            };
        }

        private static List<SectionScoreVM> GetSections(string grade)
        {
            if (grade == "پنجم" || grade == "ششم")
            {
                return new List<SectionScoreVM>
                {
                    new() { Title = "ریاضی", From = 1, To = 10 },
                    new() { Title = "علوم", From = 11, To = 20 },
                    new() { Title = "ادبیات و هوش کلامی", From = 21, To = 30 },
                    new() { Title = "هوش و استعداد", From = 31, To = 40 },
                    new() { Title = "استدلال ریاضی", From = 41, To = 45 },
                    new() { Title = "استدلال فضایی", From = 46, To = 50 },
                    new() { Title = "استدلال تصویری", From = 51, To = 55 },
                    new() { Title = "خلاقیت و حل مسئله", From = 56, To = 60 },
                };
            }

            return new List<SectionScoreVM>
            {
                new() { Title = "ریاضی", From = 1, To = 10 },
                new() { Title = "علوم", From = 11, To = 20 },
                new() { Title = "ادبیات و هوش کلامی", From = 21, To = 30 },
                new() { Title = "استدلال ریاضی", From = 31, To = 35 },
                new() { Title = "استدلال فضایی", From = 36, To = 40 },
                new() { Title = "استدلال تصویری", From = 41, To = 45 },
                new() { Title = "خلاقیت و حل مسئله", From = 46, To = 50 },
            };
        }

        private static Dictionary<int, int?> NormalizeAnswers(Dictionary<int, int?>? answers, int totalQuestions)
        {
            var normalized = new Dictionary<int, int?>();
            for (var questionNumber = 1; questionNumber <= totalQuestions; questionNumber++)
            {
                int? answer = null;
                answers?.TryGetValue(questionNumber, out answer);
                normalized[questionNumber] = answer is >= 1 and <= 4 ? answer : null;
            }

            return normalized;
        }

        private static ScoreSummary ScoreAnswers(
            Dictionary<int, int?> answers,
            Dictionary<int, int> answerKey,
            List<SectionScoreVM> sections,
            string grade)
        {
            var correct = 0;
            var wrong = 0;
            var blank = 0;
            var forcedCorrectQuestions = GetForcedCorrectQuestions(grade);

            foreach (var pair in answerKey)
            {
                answers.TryGetValue(pair.Key, out var answer);
                var section = sections.FirstOrDefault(x => pair.Key >= x.From && pair.Key <= x.To);
                if (forcedCorrectQuestions.Contains(pair.Key))
                {
                    correct++;
                    if (section != null) section.CorrectCount++;
                }
                else if (!answer.HasValue)
                {
                    blank++;
                    if (section != null) section.BlankCount++;
                }
                else if (answer.Value == pair.Value)
                {
                    correct++;
                    if (section != null) section.CorrectCount++;
                }
                else
                {
                    wrong++;
                    if (section != null) section.WrongCount++;
                }
            }

            foreach (var section in sections)
            {
                section.TotalQuestions = section.To - section.From + 1;
                section.ScorePercent = section.TotalQuestions > 0
                    ? Math.Round((decimal)section.CorrectCount * 100 / section.TotalQuestions, 2)
                    : 0;
            }

            return new ScoreSummary
            {
                CorrectCount = correct,
                WrongCount = wrong,
                BlankCount = blank,
                ScorePercent = Math.Round((decimal)correct * 100 / answerKey.Count, 2),
                Sections = sections
            };
        }

        private static HashSet<int> GetForcedCorrectQuestions(string grade)
        {
            return grade switch
            {
                "پنجم" => new HashSet<int> { 31 },
                "ششم" => new HashSet<int> { 51 },
                _ => new HashSet<int>(),
            };
        }

        private static void ApplyCurrentScoring(IEnumerable<SchoolExamResult> rows)
        {
            foreach (var row in rows)
            {
                ApplyCurrentScoring(row);
            }
        }

        private static void ApplyCurrentScoring(SchoolExamResult row)
        {
            var grade = NormalizeGrade(row.Grade, null);
            var answerKey = GetAnswerKey(grade);
            var answers = DeserializeDictionary(row.AnswersJson);
            if (answerKey.Count == 0 || answers.Count == 0)
            {
                return;
            }

            var normalizedAnswers = NormalizeAnswers(answers, answerKey.Count);
            var score = ScoreAnswers(normalizedAnswers, answerKey, GetSections(grade), grade);
            row.TotalQuestions = answerKey.Count;
            row.CorrectCount = score.CorrectCount;
            row.WrongCount = score.WrongCount;
            row.BlankCount = score.BlankCount;
            row.ScorePercent = score.ScorePercent;
            row.SectionScoresJson = JsonSerializer.Serialize(score.Sections);
        }

        private static TakinSchoolExamResultVM ToVm(SchoolExamResult row)
        {
            ApplyCurrentScoring(row);
            var user = row.SchoolRegistration.User;
            var answers = DeserializeDictionary(row.AnswersJson);
            return new TakinSchoolExamResultVM
            {
                Id = row.ID,
                SchoolRegistrationId = row.SchoolRegistrationId,
                FullName = $"{user?.FirstName} {user?.LastName}".Trim(),
                NationalCode = user?.NationalCode ?? "",
                PhoneNumber = user?.Username ?? "",
                CurrentSchoolName = row.SchoolRegistration?.CurrentSchoolName ?? "",
                Grade = row.Grade,
                TotalQuestions = row.TotalQuestions,
                CorrectCount = row.CorrectCount,
                WrongCount = row.WrongCount,
                BlankCount = row.BlankCount,
                ScorePercent = row.ScorePercent,
                IsAccepted = row.IsAccepted,
                AcceptanceMessage = row.AcceptanceMessage,
                IsFinalAccepted = row.IsFinalAccepted,
                FinalAcceptanceStatus = GetFinalAcceptanceStatus(row),
                Answers = answers,
                SectionScores = GetCurrentSectionScores(row, answers),
                EntrySource = row.EntrySource,
                OmrConfidence = row.OmrConfidence,
                OmrReviewRequired = row.OmrReviewRequired,
                Published = row.Published,
            };
        }

        private static PublicTakinSchoolExamResultVM ToPublicVm(SchoolExamResult row, List<SchoolExamResult> gradeRows)
        {
            var gradeScores = gradeRows.Select(x => x.ScorePercent).ToList();
            var rankedRows = gradeRows
                .OrderByDescending(x => x.ScorePercent)
                .ThenByDescending(x => x.CorrectCount)
                .ThenBy(x => x.WrongCount)
                .ThenBy(x => x.BlankCount)
                .ThenBy(x => x.ID)
                .ToList();

            var vm = new PublicTakinSchoolExamResultVM
            {
                Result = ToVm(row),
                AverageScorePercent = gradeScores.Count > 0 ? Math.Round(gradeScores.Average(), 2) : 0,
                HighestScorePercent = gradeScores.Count > 0 ? gradeScores.Max() : 0,
                LowestScorePercent = gradeScores.Count > 0 ? gradeScores.Min() : 0,
                ParticipantsCount = gradeScores.Count,
                Rank = rankedRows.FindIndex(x => x.ID == row.ID) + 1,
                ScoreBuckets = BuildBuckets(gradeScores),
                SectionAverages = BuildSectionAverages(gradeRows),
            };

            vm.Result.Answers = new Dictionary<int, int?>();
            return vm;
        }

        private static List<ScoreBucketVM> BuildBuckets(List<decimal> scores)
        {
            var buckets = new List<ScoreBucketVM>();
            for (var start = 0; start < 100; start += 10)
            {
                var end = start + 10;
                buckets.Add(new ScoreBucketVM
                {
                    Label = $"{start}-{end}",
                    Count = scores.Count(x => x >= start && (end == 100 ? x <= end : x < end))
                });
            }

            return buckets;
        }

        private static GradeExamAnalyticsVM BuildGradeAnalytics(string grade, List<SchoolExamResult> rows)
        {
            var scores = rows.Select(x => x.ScorePercent).ToList();
            var rankedRows = rows
                .OrderByDescending(x => x.ScorePercent)
                .ThenByDescending(x => x.CorrectCount)
                .ThenBy(x => x.WrongCount)
                .ThenBy(x => x.BlankCount)
                .ThenBy(x => x.ID)
                .ToList();

            var rankings = rankedRows
                .Select((row, index) => new StudentRankVM
                {
                    Rank = index + 1,
                    SchoolRegistrationId = row.SchoolRegistrationId,
                    FullName = $"{row.SchoolRegistration?.User?.FirstName} {row.SchoolRegistration?.User?.LastName}".Trim(),
                    NationalCode = row.SchoolRegistration?.User?.NationalCode ?? "",
                    PhoneNumber = row.SchoolRegistration?.User?.Username ?? "",
                    CurrentSchoolName = row.SchoolRegistration?.CurrentSchoolName ?? "",
                    ScorePercent = row.ScorePercent,
                    CorrectCount = row.CorrectCount,
                    WrongCount = row.WrongCount,
                    BlankCount = row.BlankCount,
                    IsAccepted = row.IsAccepted,
                    IsFinalAccepted = row.IsFinalAccepted,
                    FinalAcceptanceStatus = GetFinalAcceptanceStatus(row),
                    Published = row.Published,
                    SectionScores = GetCurrentSectionScores(row),
                })
                .ToList();

            return new GradeExamAnalyticsVM
            {
                Grade = grade,
                ParticipantsCount = rows.Count,
                PublishedCount = rows.Count(x => x.Published),
                AcceptedCount = rows.Count(x => x.IsAccepted == true),
                FinalAcceptedCount = rows.Count(x => GetFinalAcceptanceStatus(x) == FinalStatusAccepted),
                AverageScorePercent = rows.Count > 0 ? Math.Round(rows.Average(x => x.ScorePercent), 2) : 0,
                HighestScorePercent = rows.Count > 0 ? rows.Max(x => x.ScorePercent) : 0,
                LowestScorePercent = rows.Count > 0 ? rows.Min(x => x.ScorePercent) : 0,
                ScoreBuckets = BuildBuckets(scores),
                SectionAverages = BuildSectionAverages(rows),
                Rankings = rankings,
            };
        }

        private static List<SectionAverageVM> BuildSectionAverages(List<SchoolExamResult> rows)
        {
            return rows
                .SelectMany(GetCurrentSectionScores)
                .GroupBy(section => new { section.Title, section.From, section.To })
                .OrderBy(group => group.Key.From)
                .Select(group => new SectionAverageVM
                {
                    Title = group.Key.Title,
                    From = group.Key.From,
                    To = group.Key.To,
                    AverageScorePercent = Math.Round(group.Average(x => x.ScorePercent), 2),
                    AverageCorrectCount = Math.Round(group.Average(x => (decimal)x.CorrectCount), 2),
                    AverageWrongCount = Math.Round(group.Average(x => (decimal)x.WrongCount), 2),
                    AverageBlankCount = Math.Round(group.Average(x => (decimal)x.BlankCount), 2),
                })
                .ToList();
        }

        private static List<SectionScoreVM> GetCurrentSectionScores(SchoolExamResult row)
        {
            return GetCurrentSectionScores(row, DeserializeDictionary(row.AnswersJson));
        }

        private static List<SectionScoreVM> GetCurrentSectionScores(SchoolExamResult row, Dictionary<int, int?> answers)
        {
            var grade = NormalizeGrade(row.Grade, null);
            var answerKey = GetAnswerKey(grade);
            if (answerKey.Count == 0 || answers.Count == 0)
            {
                return DeserializeSections(row.SectionScoresJson);
            }

            var normalizedAnswers = NormalizeAnswers(answers, answerKey.Count);
            return ScoreAnswers(normalizedAnswers, answerKey, GetSections(grade), grade).Sections;
        }

        private static int GradeSortIndex(string grade)
        {
            return grade switch
            {
                "چهارم" => 1,
                "پنجم" => 2,
                "ششم" => 3,
                _ => 99,
            };
        }

        private static Dictionary<int, int?> DeserializeDictionary(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<int, int?>>(json) ?? new Dictionary<int, int?>();
            }
            catch
            {
                return new Dictionary<int, int?>();
            }
        }

        private static List<SectionScoreVM> DeserializeSections(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<SectionScoreVM>>(json) ?? new List<SectionScoreVM>();
            }
            catch
            {
                return new List<SectionScoreVM>();
            }
        }

        private static string NormalizeGrade(string? requestedGrade, string? fallbackGrade)
        {
            var grade = (requestedGrade ?? fallbackGrade ?? "").Trim();
            return string.IsNullOrWhiteSpace(grade) ? "چهارم" : grade;
        }

        private static string OnlyDigits(string? value)
        {
            return new string((value ?? "").Where(char.IsDigit).ToArray());
        }

        private static string GetFinalAcceptanceStatus(SchoolExamResult row)
        {
            return NormalizeFinalAcceptanceStatus(row.FinalAcceptanceStatus, row.IsFinalAccepted);
        }

        private static string NormalizeFinalAcceptanceStatus(string? status, bool? legacyValue)
        {
            var normalized = (status ?? "").Trim().ToLowerInvariant();
            return normalized switch
            {
                FinalStatusAccepted => FinalStatusAccepted,
                FinalStatusRejected => FinalStatusRejected,
                FinalStatusReserved => FinalStatusReserved,
                FinalStatusPending => FinalStatusPending,
                "reserve" => FinalStatusReserved,
                "waitlist" => FinalStatusReserved,
                "waiting" => FinalStatusReserved,
                "true" => FinalStatusAccepted,
                "false" => FinalStatusRejected,
                _ => legacyValue switch
                {
                    true => FinalStatusAccepted,
                    false => FinalStatusRejected,
                    _ => FinalStatusPending,
                },
            };
        }

        private static bool? ToLegacyFinalAccepted(string status)
        {
            return status switch
            {
                FinalStatusAccepted => true,
                FinalStatusRejected => false,
                _ => null,
            };
        }
    }

    public class GetSchoolExamResultByRegistrationRequestBody
    {
        public long SchoolRegistrationId { get; set; }
        public string Grade { get; set; } = "چهارم";
    }

    public class SaveTakinSchoolExamResultRequestBody
    {
        public long SchoolRegistrationId { get; set; }
        public string Grade { get; set; } = "چهارم";
        public Dictionary<int, int?> Answers { get; set; } = new();
        public bool? IsAccepted { get; set; }
        public string? AcceptanceMessage { get; set; }
        public string EntrySource { get; set; } = "Manual";
        public decimal? OmrConfidence { get; set; }
        public bool OmrReviewRequired { get; set; }
        public string? RawOmrJson { get; set; }
        public bool Published { get; set; }
    }

    public class PublicTakinSchoolExamResultRequestBody
    {
        public string PhoneNumber { get; set; } = "";
        public string NationalCode { get; set; } = "";
    }

    public class SetTakinSchoolExamInterviewStatusRequestBody
    {
        public long SchoolRegistrationId { get; set; }
        public string Grade { get; set; } = "چهارم";
        public bool? InvitedToInterview { get; set; }
        public string? Message { get; set; }
    }

    public class SetTakinSchoolExamFinalAcceptanceStatusRequestBody
    {
        public long SchoolRegistrationId { get; set; }
        public string Grade { get; set; } = "چهارم";
        public bool? IsFinalAccepted { get; set; }
        public string? FinalAcceptanceStatus { get; set; }
    }

    public class BulkUpdateTakinSchoolExamResultsRequestBody
    {
        public List<long> SchoolRegistrationIds { get; set; } = new();
        public string Grade { get; set; } = "چهارم";
        public bool UpdateInterviewStatus { get; set; }
        public bool? InvitedToInterview { get; set; }
        public bool UpdateFinalAcceptanceStatus { get; set; }
        public bool? IsFinalAccepted { get; set; }
        public string? FinalAcceptanceStatus { get; set; }
        public bool UpdatePublished { get; set; }
        public bool Published { get; set; }
    }

    public class TakinSchoolExamResultVM
    {
        public long Id { get; set; }
        public long SchoolRegistrationId { get; set; }
        public string FullName { get; set; } = "";
        public string NationalCode { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string CurrentSchoolName { get; set; } = "";
        public string Grade { get; set; } = "";
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int BlankCount { get; set; }
        public decimal ScorePercent { get; set; }
        public bool? IsAccepted { get; set; }
        public string AcceptanceMessage { get; set; } = "";
        public bool? IsFinalAccepted { get; set; }
        public string FinalAcceptanceStatus { get; set; } = "";
        public Dictionary<int, int?> Answers { get; set; } = new();
        public List<SectionScoreVM> SectionScores { get; set; } = new();
        public string EntrySource { get; set; } = "";
        public decimal? OmrConfidence { get; set; }
        public bool OmrReviewRequired { get; set; }
        public bool Published { get; set; }
    }

    public class PublicTakinSchoolExamResultVM
    {
        public TakinSchoolExamResultVM Result { get; set; } = new();
        public decimal AverageScorePercent { get; set; }
        public decimal HighestScorePercent { get; set; }
        public decimal LowestScorePercent { get; set; }
        public int ParticipantsCount { get; set; }
        public int Rank { get; set; }
        public List<ScoreBucketVM> ScoreBuckets { get; set; } = new();
        public List<SectionAverageVM> SectionAverages { get; set; } = new();
    }

    public class TakinSchoolExamAnalyticsVM
    {
        public int TotalParticipants { get; set; }
        public int PublishedCount { get; set; }
        public int AcceptedCount { get; set; }
        public int PendingAcceptanceCount { get; set; }
        public int FinalAcceptedCount { get; set; }
        public int PendingFinalAcceptanceCount { get; set; }
        public decimal AverageScorePercent { get; set; }
        public decimal HighestScorePercent { get; set; }
        public List<GradeExamAnalyticsVM> Grades { get; set; } = new();
    }

    public class GradeExamAnalyticsVM
    {
        public string Grade { get; set; } = "";
        public int ParticipantsCount { get; set; }
        public int PublishedCount { get; set; }
        public int AcceptedCount { get; set; }
        public int FinalAcceptedCount { get; set; }
        public decimal AverageScorePercent { get; set; }
        public decimal HighestScorePercent { get; set; }
        public decimal LowestScorePercent { get; set; }
        public List<ScoreBucketVM> ScoreBuckets { get; set; } = new();
        public List<SectionAverageVM> SectionAverages { get; set; } = new();
        public List<StudentRankVM> Rankings { get; set; } = new();
    }

    public class StudentRankVM
    {
        public int Rank { get; set; }
        public long SchoolRegistrationId { get; set; }
        public string FullName { get; set; } = "";
        public string NationalCode { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string CurrentSchoolName { get; set; } = "";
        public decimal ScorePercent { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int BlankCount { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? IsFinalAccepted { get; set; }
        public string FinalAcceptanceStatus { get; set; } = "";
        public bool Published { get; set; }
        public List<SectionScoreVM> SectionScores { get; set; } = new();
    }

    public class SectionAverageVM
    {
        public string Title { get; set; } = "";
        public int From { get; set; }
        public int To { get; set; }
        public decimal AverageScorePercent { get; set; }
        public decimal AverageCorrectCount { get; set; }
        public decimal AverageWrongCount { get; set; }
        public decimal AverageBlankCount { get; set; }
    }

    public class SectionScoreVM
    {
        public string Title { get; set; } = "";
        public int From { get; set; }
        public int To { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int BlankCount { get; set; }
        public decimal ScorePercent { get; set; }
    }

    public class ScoreBucketVM
    {
        public string Label { get; set; } = "";
        public int Count { get; set; }
    }

    internal class ScoreSummary
    {
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int BlankCount { get; set; }
        public decimal ScorePercent { get; set; }
        public List<SectionScoreVM> Sections { get; set; } = new();
    }
}
