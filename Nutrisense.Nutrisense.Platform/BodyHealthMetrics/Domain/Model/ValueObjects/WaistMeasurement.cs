namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Validated waist circumference in centimetres, enforcing a range of 30–250 cm.</summary>
public sealed record WaistMeasurement
{
    public decimal Cm { get; }

    public WaistMeasurement(decimal cm)
    {
        if (cm < 30m || cm > 250m)
            throw new ArgumentException("Waist measurement must be between 30 and 250 cm.", nameof(cm));
        Cm = cm;
    }
}
