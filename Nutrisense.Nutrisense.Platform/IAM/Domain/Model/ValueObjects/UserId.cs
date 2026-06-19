namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record UserId
{
    public int Value { get; }

    public UserId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("UserId must be a positive integer.", nameof(value));
        Value = value;
    }

    // Bypasses domain validation — use only from EF Core value converters.
    internal static UserId FromRaw(int value) => new(value, skipValidation: true);

    private UserId(int value, bool skipValidation)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(UserId id) => id.Value;
    public static explicit operator UserId(int value) => new(value);
}
