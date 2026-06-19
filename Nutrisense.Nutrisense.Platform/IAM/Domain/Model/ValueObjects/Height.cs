namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

public sealed record Height
{
    public decimal Centimeters { get; }

    public Height(decimal centimeters)
    {
        if (centimeters < 50 || centimeters > 272)
            throw new ArgumentException(
                $"Height must be between 50 and 272 cm, but was {centimeters}.",
                nameof(centimeters));
        Centimeters = centimeters;
    }
}
