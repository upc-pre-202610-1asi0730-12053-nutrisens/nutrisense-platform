namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record SessionResource(
    int Id,
    string DeviceLabel,
    bool IsCurrent,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastActiveAt)
{
    /// <summary>Unique identifier for the session.</summary>
    public int Id { get; init; } = Id;

    /// <summary>User-provided or system-generated label identifying the device/client (e.g., "iPhone 14", "Desktop").</summary>
    public string DeviceLabel { get; init; } = DeviceLabel;

    /// <summary>Indicates whether this session corresponds to the current authentication token.</summary>
    public bool IsCurrent { get; init; } = IsCurrent;

    /// <summary>Timestamp when the session was created. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    public DateTimeOffset CreatedAt { get; init; } = CreatedAt;

    /// <summary>Timestamp of the last API activity for this session. Format: ISO 8601 (yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    public DateTimeOffset LastActiveAt { get; init; } = LastActiveAt;
}
