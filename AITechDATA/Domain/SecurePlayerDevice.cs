namespace AITechDATA.Domain;

public class SecurePlayerDevice : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public string DeviceFingerprint { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public DateTime FirstActivatedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
