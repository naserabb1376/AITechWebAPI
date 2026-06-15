using AITechDATA.DataLayer;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechWebAPI.Models.SecurePlayer;
using AITechWebAPI.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AITechWebAPI.Controllers;

[Route("SecurePlayer")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class SecurePlayerController : ControllerBase
{
    private readonly AITechContext _db;
    private readonly ILogger<SecurePlayerController> _logger;

    public SecurePlayerController(AITechContext db, ILogger<SecurePlayerController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpPost("MyLibrary")]
    public async Task<ActionResult<RowResultObject<SecurePlayerLibraryResponse>>> MyLibrary(SecurePlayerLibraryRequest request)
    {
        var result = new RowResultObject<SecurePlayerLibraryResponse>();
        var userId = User.GetCurrentUserId();
        _logger.LogInformation("SecurePlayer MyLibrary requested. UserId={UserId}, DeviceId={DeviceId}, DeviceName={DeviceName}",
            userId,
            SafeDevice(request.DeviceId),
            request.DeviceName);

        var deviceCheck = await EnsureDeviceAsync(userId, request.DeviceId, request.DeviceName);

        if (!deviceCheck.Status)
        {
            _logger.LogWarning("SecurePlayer MyLibrary rejected. UserId={UserId}, DeviceId={DeviceId}, Reason={Reason}",
                userId,
                SafeDevice(request.DeviceId),
                deviceCheck.ErrorMessage);
            result.Status = false;
            result.ErrorMessage = deviceCheck.ErrorMessage;
            return BadRequest(result);
        }

        var userCourses = await _db.UserCourses
            .AsNoTracking()
            .Where(x => x.IsActive && x.UserId == userId)
            .Include(x => x.Course)
            .ToListAsync();

        var userGroups = await _db.UserGroups
            .AsNoTracking()
            .Where(x => x.IsActive && x.UserId == userId)
            .Include(x => x.Group)
                .ThenInclude(x => x.Course)
            .Include(x => x.Group)
                .ThenInclude(x => x.Teacher)
            .Include(x => x.Group)
                .ThenInclude(x => x.Sessions)
            .ToListAsync();

        var courses = userCourses
            .Select(x => new SecurePlayerCourseResponse
            {
                Id = x.CourseId,
                Title = x.Course.Title,
                Teacher = "AITech",
                Description = x.Course.Description ?? "",
                ProgressLabel = "۰٪",
                LicenseStatus = "فعال روی این دستگاه"
            })
            .ToDictionary(x => x.Id, x => x);

        foreach (var userGroup in userGroups)
        {
            var group = userGroup.Group;
            if (!courses.TryGetValue(group.CourseId, out var course))
            {
                course = new SecurePlayerCourseResponse
                {
                    Id = group.CourseId,
                    Title = group.Course.Title,
                    Teacher = $"مدرس: {group.Teacher.FirstName} {group.Teacher.LastName}".Trim(),
                    Description = group.Course.Description ?? group.Name,
                    ProgressLabel = "۰٪",
                    LicenseStatus = "فعال روی این دستگاه"
                };
                courses.Add(course.Id, course);
            }

            foreach (var session in group.Sessions.OrderBy(x => x.SessionDate).ThenBy(x => x.ID))
            {
                if (course.Sessions.Any(x => x.Id == session.ID))
                {
                    continue;
                }

                course.Sessions.Add(new SecurePlayerSessionResponse
                {
                    Id = session.ID,
                    Title = string.IsNullOrWhiteSpace(session.Description)
                        ? $"جلسه {course.Sessions.Count + 1}"
                        : session.Description,
                    Duration = FormatDuration(session.VideoDurationSeconds),
                    IsCompleted = false,
                    IsOnlineOnly = true,
                    PlaybackToken = ""
                });
            }
        }

        result.Status = true;
        result.Result = new SecurePlayerLibraryResponse
        {
            DeviceStatus = deviceCheck.Result ?? "active",
            Courses = courses.Values.OrderBy(x => x.Title).ToList()
        };

        _logger.LogInformation("SecurePlayer MyLibrary completed. UserId={UserId}, Courses={CourseCount}",
            userId,
            result.Result.Courses.Count);

        return Ok(result);
    }

    [HttpPost("StartPlayback")]
    public async Task<ActionResult<RowResultObject<SecurePlayerPlaybackResponse>>> StartPlayback(SecurePlayerSessionRequest request)
    {
        var result = new RowResultObject<SecurePlayerPlaybackResponse>();
        var userId = User.GetCurrentUserId();
        _logger.LogInformation("SecurePlayer StartPlayback requested. UserId={UserId}, SessionId={SessionId}, DeviceId={DeviceId}",
            userId,
            request.SessionId,
            SafeDevice(request.DeviceId));

        var deviceCheck = await EnsureDeviceAsync(userId, request.DeviceId, request.DeviceName);

        if (!deviceCheck.Status)
        {
            _logger.LogWarning("SecurePlayer StartPlayback rejected by device lock. UserId={UserId}, SessionId={SessionId}, DeviceId={DeviceId}, Reason={Reason}",
                userId,
                request.SessionId,
                SafeDevice(request.DeviceId),
                deviceCheck.ErrorMessage);
            result.Status = false;
            result.ErrorMessage = deviceCheck.ErrorMessage;
            return BadRequest(result);
        }

        var allowed = await _db.UserGroups
            .AsNoTracking()
            .AnyAsync(x => x.IsActive
                && x.UserId == userId
                && x.Group.Sessions.Any(s => s.ID == request.SessionId));

        if (!allowed)
        {
            _logger.LogWarning("SecurePlayer StartPlayback rejected by access check. UserId={UserId}, SessionId={SessionId}, DeviceId={DeviceId}",
                userId,
                request.SessionId,
                SafeDevice(request.DeviceId));
            result.Status = false;
            result.ErrorMessage = "این جلسه برای حساب شما فعال نیست.";
            return BadRequest(result);
        }

        _db.SecurePlayerViewLogs.Add(new SecurePlayerViewLog
        {
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            UserId = userId,
            SessionId = request.SessionId,
            DeviceFingerprint = request.DeviceId,
            Action = "StartPlayback",
            LoggedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();

        var expiresAt = DateTime.Now.AddMinutes(10);
        result.Status = true;
        result.Result = new SecurePlayerPlaybackResponse
        {
            SessionId = request.SessionId,
            PlaybackToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            ExpiresAt = expiresAt,
            StreamUrl = ""
        };

        _logger.LogInformation("SecurePlayer StartPlayback completed. UserId={UserId}, SessionId={SessionId}, ExpiresAt={ExpiresAt}",
            userId,
            request.SessionId,
            expiresAt);

        return Ok(result);
    }

    private async Task<RowResultObject<string>> EnsureDeviceAsync(long userId, string deviceId, string deviceName)
    {
        var result = new RowResultObject<string>();
        if (string.IsNullOrWhiteSpace(deviceId) || deviceId.Length < 16)
        {
            _logger.LogWarning("SecurePlayer device validation failed. UserId={UserId}, DeviceId={DeviceId}",
                userId,
                SafeDevice(deviceId));
            result.Status = false;
            result.ErrorMessage = "شناسه دستگاه معتبر نیست.";
            return result;
        }

        var devices = await _db.SecurePlayerDevices
            .Where(x => x.UserId == userId && x.IsActive && x.RevokedAt == null)
            .ToListAsync();

        var currentDevice = devices.FirstOrDefault(x => x.DeviceFingerprint == deviceId);
        if (currentDevice is null && devices.Any())
        {
            _logger.LogWarning("SecurePlayer second device blocked. UserId={UserId}, RequestedDeviceId={RequestedDeviceId}, ExistingDevices={DeviceCount}",
                userId,
                SafeDevice(deviceId),
                devices.Count);
            result.Status = false;
            result.ErrorMessage = "این حساب قبلا روی دستگاه دیگری فعال شده است. برای تغییر دستگاه با پشتیبانی تماس بگیرید.";
            return result;
        }

        if (currentDevice is null)
        {
            currentDevice = new SecurePlayerDevice
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                UserId = userId,
                DeviceFingerprint = deviceId,
                DeviceName = string.IsNullOrWhiteSpace(deviceName) ? "AITech Player Device" : deviceName,
                FirstActivatedAt = DateTime.Now,
                LastSeenAt = DateTime.Now,
                IsActive = true
            };
            _db.SecurePlayerDevices.Add(currentDevice);
            await _db.SaveChangesAsync();
            _logger.LogInformation("SecurePlayer device activated. UserId={UserId}, DeviceId={DeviceId}, DeviceName={DeviceName}",
                userId,
                SafeDevice(deviceId),
                currentDevice.DeviceName);
            result.Result = "activated";
        }
        else
        {
            currentDevice.LastSeenAt = DateTime.Now;
            currentDevice.UpdateDate = DateTime.Now;
            await _db.SaveChangesAsync();
            result.Result = "active";
        }

        result.Status = true;
        return result;
    }

    private static string FormatDuration(int? seconds)
    {
        if (!seconds.HasValue || seconds.Value <= 0)
        {
            return "--:--";
        }

        var time = TimeSpan.FromSeconds(seconds.Value);
        return time.TotalHours >= 1
            ? $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}"
            : $"{time.Minutes:00}:{time.Seconds:00}";
    }

    private static string SafeDevice(string? deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return "";
        }

        return deviceId.Length <= 12
            ? deviceId
            : $"{deviceId[..8]}...{deviceId[^4..]}";
    }
}
