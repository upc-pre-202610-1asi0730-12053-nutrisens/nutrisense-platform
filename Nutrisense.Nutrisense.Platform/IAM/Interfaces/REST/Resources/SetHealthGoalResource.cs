namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record SetHealthGoalResource(string Goal)
{
    /// <summary>Health goal intent. Valid values: Bulk, Cut, Maintain, Lose.</summary>
    public string Goal { get; init; } = Goal;
}
