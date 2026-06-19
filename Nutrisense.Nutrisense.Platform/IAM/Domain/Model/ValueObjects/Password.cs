namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password cannot be empty.", nameof(value));
        if (value.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(value));
        if (!value.Any(char.IsLetter))
            throw new ArgumentException("Password must contain at least one letter.", nameof(value));
        if (!value.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one digit.", nameof(value));
        Value = value;
    }
}
