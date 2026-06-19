namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record PersonName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FullName => $"{FirstName} {LastName}";

    public PersonName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (firstName.Length > 80)
            throw new ArgumentException("First name exceeds maximum length of 80.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        if (lastName.Length > 80)
            throw new ArgumentException("Last name exceeds maximum length of 80.", nameof(lastName));
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }
}
