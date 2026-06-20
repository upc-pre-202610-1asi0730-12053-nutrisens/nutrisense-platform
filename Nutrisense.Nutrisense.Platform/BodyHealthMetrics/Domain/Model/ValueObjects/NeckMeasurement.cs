namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Validated neck circumference in centimetres, enforcing a range of 20–80 cm.</summary>
public sealed record NeckMeasurement
{
    public decimal Cm { get; }

    public NeckMeasurement(decimal cm)
    {
        if (cm != 0 && (cm < 20m || cm > 80m))
            throw new ArgumentException("Neck measurement must be between 20 and 80 cm.", nameof(cm));
        Cm = cm;
    }
}
