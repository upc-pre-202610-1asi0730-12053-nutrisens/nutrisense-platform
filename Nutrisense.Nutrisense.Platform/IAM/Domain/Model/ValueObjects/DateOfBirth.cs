namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record DateOfBirth
{
    public DateOnly Value { get; }

    public DateOfBirth(DateOnly value)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (value >= today)
            throw new ArgumentException("Date of birth must be in the past.", nameof(value));

        var age = today.Year - value.Year;
        if (today < value.AddYears(age)) age--;

        if (age < 13)
            throw new ArgumentException("User must be at least 13 years old.", nameof(value));
        if (age > 120)
            throw new ArgumentException("Date of birth is not plausible (age > 120).", nameof(value));

        Value = value;
    }
}
