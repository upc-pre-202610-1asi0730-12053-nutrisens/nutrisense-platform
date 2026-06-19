using System.Text.RegularExpressions;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record Email
{
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        if (value.Length > 255)
            throw new ArgumentException("Email exceeds maximum length of 255.", nameof(value));
        if (!Pattern.IsMatch(value))
            throw new ArgumentException($"'{value}' is not a valid email address.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }
}
