namespace AITechDATA.Domain;

public class SecurePlayerViewLog : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public long SessionId { get; set; }
    public Session Session { get; set; } = null!;
    public string DeviceFingerprint { get; set; } = "";
    public string Action { get; set; } = "";
    public DateTime LoggedAt { get; set; }
}
