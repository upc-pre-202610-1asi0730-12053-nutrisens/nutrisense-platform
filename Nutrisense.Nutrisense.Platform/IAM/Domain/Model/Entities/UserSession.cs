using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;

public class UserSession
{
    public int Id { get; private set; }
    public UserId UserId { get; private set; } = null!;
    public string DeviceLabel { get; private set; }
    public bool IsCurrent { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset LastActiveAt { get; private set; }

    protected UserSession()
    {
        DeviceLabel = string.Empty;
    }

    public UserSession(UserId userId, string deviceLabel)
    {
        UserId = userId;
        DeviceLabel = deviceLabel;
        IsCurrent = true;
        CreatedAt = DateTimeOffset.UtcNow;
        LastActiveAt = DateTimeOffset.UtcNow;
    }

    public void End()
    {
        IsCurrent = false;
        LastActiveAt = DateTimeOffset.UtcNow;
    }
}
