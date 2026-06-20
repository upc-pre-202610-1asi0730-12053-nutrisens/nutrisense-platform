namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be >= 0.");
        if (string.IsNullOrEmpty(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-character code.");
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }
}
