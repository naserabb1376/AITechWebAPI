using AITechDATA.DataLayer;
using AITechDATA.Domain;
using AITechDATA.Tools;
using AITechWebAPI.Models.AttendanceDeviceSync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITechWebAPI.Controllers
{
    [Route("AttendanceDeviceSync")]
    [ApiController]
    [Produces("application/json")]
    public class AttendanceDeviceSyncController : ControllerBase
    {
        private const string ApiKeyHeaderName = "X-Device-Sync-Key";
        private readonly AITechContext _context;
        private readonly IConfiguration _configuration;

        public AttendanceDeviceSyncController(AITechContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("SyncTimeFunctions")]
        public async Task<ActionResult<SyncDeviceAttendanceResultBody>> SyncTimeFunctions(SyncDeviceAttendanceRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(requestBody);
            }

            var configuredApiKey = _configuration["AttendanceDeviceSync:ApiKey"];
            if (string.IsNullOrWhiteSpace(configuredApiKey))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Attendance device sync API key is not configured.");
            }

            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var requestApiKey) || requestApiKey != configuredApiKey)
            {
                return Unauthorized();
            }

            var result = new SyncDeviceAttendanceResultBody
            {
                ReceivedCount = requestBody.Logs.Count
            };

            var normalizedLogs = requestBody.Logs
                .Where(x => !string.IsNullOrWhiteSpace(x.EnrollNumber) && x.Timestamp != default)
                .Select(x => new DeviceLog(
                    x.EnrollNumber.Trim(),
                    x.Timestamp,
                    x.VerifyMode,
                    x.InOutMode,
                    x.WorkCode))
                .ToList();

            if (!normalizedLogs.Any())
            {
                return Ok(result);
            }

            var deviceSerial = string.IsNullOrWhiteSpace(requestBody.DeviceSerial)
                ? "default"
                : requestBody.DeviceSerial.Trim();

            var deviceUserIds = normalizedLogs
                .Select(x => x.EnrollNumber)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            var users = await _context.Users
                .Where(x => x.AttendanceDeviceUserId != null && deviceUserIds.Contains(x.AttendanceDeviceUserId))
                .ToListAsync();

            var usersByDeviceId = users
                .GroupBy(x => x.AttendanceDeviceUserId!, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);

            result.UnmatchedDeviceUserIds = deviceUserIds
                .Where(x => !usersByDeviceId.ContainsKey(x))
                .OrderBy(x => x)
                .ToList();

            var matchedLogs = normalizedLogs
                .Where(x => usersByDeviceId.ContainsKey(x.EnrollNumber))
                .ToList();

            result.MatchedLogCount = matchedLogs.Count;

            var groups = matchedLogs
                .GroupBy(x => new
                {
                    UserId = usersByDeviceId[x.EnrollNumber].ID,
                    SourceDeviceDate = x.Timestamp.Date
                })
                .ToList();

            foreach (var group in groups)
            {
                var orderedLogs = group.OrderBy(x => x.Timestamp).ToList();
                if (orderedLogs.Count < 2)
                {
                    result.SkippedSinglePunchDays++;
                    continue;
                }

                var startLog = orderedLogs.First();
                var endLog = orderedLogs.Last();
                if (endLog.Timestamp <= startLog.Timestamp)
                {
                    result.SkippedSinglePunchDays++;
                    continue;
                }

                var existing = await _context.TimeFunctions
                    .SingleOrDefaultAsync(x =>
                        x.UserId == group.Key.UserId &&
                        x.SourceDeviceSerial == deviceSerial &&
                        x.SourceDeviceDate == group.Key.SourceDeviceDate);

                if (existing == null)
                {
                    var timeFunction = new TimeFunction
                    {
                        CreateDate = DateTime.Now.ToShamsi(),
                        UpdateDate = DateTime.Now.ToShamsi(),
                        UserId = group.Key.UserId,
                        TimeFunctionStartDate = startLog.Timestamp,
                        TimeFunctionEndDate = endLog.Timestamp,
                        Description = "ثبت خودکار از دستگاه حضور و غیاب",
                        SourceDeviceSerial = deviceSerial,
                        SourceDeviceDate = group.Key.SourceDeviceDate,
                        SourceDeviceStartLogKey = startLog.GetKey(deviceSerial),
                        SourceDeviceEndLogKey = endLog.GetKey(deviceSerial),
                        IsActive = true,
                        OtherLangs = ""
                    };

                    await _context.TimeFunctions.AddAsync(timeFunction);
                    result.InsertedCount++;
                }
                else
                {
                    existing.UpdateDate = DateTime.Now.ToShamsi();
                    existing.TimeFunctionStartDate = startLog.Timestamp;
                    existing.TimeFunctionEndDate = endLog.Timestamp;
                    existing.SourceDeviceStartLogKey = startLog.GetKey(deviceSerial);
                    existing.SourceDeviceEndLogKey = endLog.GetKey(deviceSerial);
                    existing.Description = string.IsNullOrWhiteSpace(existing.Description)
                        ? "ثبت خودکار از دستگاه حضور و غیاب"
                        : existing.Description;
                    result.UpdatedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(result);
        }

        private sealed record DeviceLog(
            string EnrollNumber,
            DateTime Timestamp,
            int VerifyMode,
            int InOutMode,
            int WorkCode)
        {
            public string GetKey(string deviceSerial)
            {
                return $"{deviceSerial}|{EnrollNumber}|{Timestamp:O}|{VerifyMode}|{InOutMode}|{WorkCode}";
            }
        }
    }
}
