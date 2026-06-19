namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.Acl;

/// <summary>
/// Public Anti-Corruption-Layer contract for the IAM bounded context.
/// Other BCs consume identity data through this facade without coupling to IAM's
/// domain model: every parameter and return value is a primitive or a primitive-only
/// collection — never a Command, aggregate or entity. Every method degrades gracefully,
/// returning an empty collection or null on failure instead of throwing.
/// </summary>
public interface IIamContextFacade
{
    /// <summary>
    /// Returns the dietary restriction labels configured by a user.
    /// Empty if the user has none, does not exist, or on failure.
    /// </summary>
    Task<IReadOnlyList<string>> GetDietaryRestrictionsByUserId(int userId, CancellationToken ct = default);
}
