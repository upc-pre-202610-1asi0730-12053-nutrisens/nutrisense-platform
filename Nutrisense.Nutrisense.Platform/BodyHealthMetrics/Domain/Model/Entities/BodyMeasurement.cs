using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;

/// <summary>Timestamped circumference snapshot (waist and neck) recorded for a user.</summary>
public class BodyMeasurement
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public WaistMeasurement WaistCm { get; private set; } = null!;
    public NeckMeasurement NeckCm { get; private set; } = null!;
    public DateTimeOffset MeasuredAt { get; private set; }

    protected BodyMeasurement() { }

    public BodyMeasurement(int userId, WaistMeasurement waistCm, NeckMeasurement neckCm, DateTimeOffset measuredAt)
    {
        UserId = userId;
        WaistCm = waistCm;
        NeckCm = neckCm;
        MeasuredAt = measuredAt;
    }
}
