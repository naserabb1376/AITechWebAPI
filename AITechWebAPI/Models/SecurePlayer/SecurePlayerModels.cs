namespace AITechWebAPI.Models.SecurePlayer;

public class SecurePlayerLibraryRequest
{
    public string DeviceId { get; set; } = "";
    public string DeviceName { get; set; } = "";
}

public sealed class SecurePlayerSessionRequest : SecurePlayerLibraryRequest
{
    public long SessionId { get; set; }
}

public sealed class SecurePlayerLibraryResponse
{
    public string DeviceStatus { get; set; } = "";
    public List<SecurePlayerCourseResponse> Courses { get; set; } = new();
}

public sealed class SecurePlayerCourseResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Teacher { get; set; } = "";
    public string Description { get; set; } = "";
    public string ProgressLabel { get; set; } = "";
    public string LicenseStatus { get; set; } = "";
    public List<SecurePlayerSessionResponse> Sessions { get; set; } = new();
}

public sealed class SecurePlayerSessionResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Duration { get; set; } = "";
    public bool IsCompleted { get; set; }
    public bool IsOnlineOnly { get; set; } = true;
    public string PlaybackToken { get; set; } = "";
}

public sealed class SecurePlayerPlaybackResponse
{
    public long SessionId { get; set; }
    public string PlaybackToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public string StreamUrl { get; set; } = "";
}
