namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

public sealed record CardExpiry
{
    public int Month { get; }
    public int Year { get; }

    public CardExpiry(int month, int year)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Month must be between 1 and 12.");

        var now = DateTime.UtcNow;
        if (year < now.Year || (year == now.Year && month < now.Month))
            throw new ArgumentException("Card is expired.");

        Month = month;
        Year = year;
    }
}
