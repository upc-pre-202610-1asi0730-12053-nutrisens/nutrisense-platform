using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;

public class DietaryRestriction
{
    public int Id { get; private set; }
    public UserId UserId { get; private set; } = null!;
    public string Restriction { get; private set; }

    protected DietaryRestriction()
    {
        Restriction = string.Empty;
    }

    public DietaryRestriction(UserId userId, string restriction)
    {
        UserId = userId;
        Restriction = restriction;
    }
}
