using System.Globalization;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps RegisterBodyMetricsResource to RegisterBodyMetricsCommand, parsing the date-of-birth string.</summary>
public static class RegisterBodyMetricsCommandAssembler
{
    public static RegisterBodyMetricsCommand ToCommand(RegisterBodyMetricsResource resource)
    {
        if (!DateOnly.TryParseExact(resource.DateOfBirth, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
            throw new ArgumentException("Invalid date of birth format. Expected yyyy-MM-dd.", nameof(resource));

        return new RegisterBodyMetricsCommand(
            resource.UserId,
            resource.HeightCm,
            resource.WeightKg,
            dateOfBirth,
            resource.BiologicalSex,
            resource.ActivityLevel,
            resource.Goal,
            resource.WeeklyRateKg);
    }
}
