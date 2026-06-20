namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.ValueObjects;

/// <summary>Value object representing an inclusive, validated span between two calendar dates.</summary>
public sealed record DateRange
{
    public DateOnly From { get; }
    public DateOnly To { get; }

    public DateRange(DateOnly from, DateOnly to)
    {
        if (to < from)
            throw new ArgumentException("DateRange 'To' must be greater than or equal to 'From'.");
        if ((to.ToDateTime(TimeOnly.MinValue) - from.ToDateTime(TimeOnly.MinValue)).TotalDays > 366)
            throw new ArgumentException("DateRange cannot exceed 366 days.");
        From = from;
        To = to;
    }
}
