namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

/// <summary>
/// A geocoding match for a place: localized names, ISO country code, optional state/region,
/// and coordinates. <see cref="NameEn"/>/<see cref="NameEs"/> fall back to <see cref="Name"/>
/// when the provider has no localized variant.
/// </summary>
public record GeoCityCandidate(
    string Name,
    string? NameEn,
    string? NameEs,
    string Country,
    string? State,
    decimal Lat,
    decimal Lng);

/// <summary>
/// Resolves places from a geocoding provider. Used to look up cities the local catalog
/// does not yet know about (forward, by name) and to detect a city from coordinates (reverse).
/// </summary>
public interface IGeocodingService
{
    Task<IReadOnlyList<GeoCityCandidate>> SearchAsync(string query, int limit, CancellationToken ct = default);
    Task<GeoCityCandidate?> ReverseAsync(decimal lat, decimal lng, CancellationToken ct = default);
}
