using System.Globalization;
using System.Text;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

public class City
{
    public int Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string NameEs { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public decimal Lat { get; private set; }
    public decimal Lng { get; private set; }
    public string Timezone { get; private set; } = null!;

    protected City() { }

    public City(string key, string nameEn, string nameEs, string country, decimal lat, decimal lng, string timezone)
    {
        Key = key;
        NameEn = nameEn;
        NameEs = nameEs;
        Country = country;
        Lat = lat;
        Lng = lng;
        Timezone = timezone;
    }

    /// <summary>
    /// Builds a city imported from geocoding. The <see cref="Key"/> embeds rounded coordinates so
    /// distinct localities that share a name+country never collapse onto the same key (which would
    /// otherwise make the unique index silently resolve to the wrong city). Timezone is left as a
    /// "UTC" placeholder since geocoding does not provide one.
    /// </summary>
    public static City Import(string name, string? nameEn, string? nameEs, string country, decimal lat, decimal lng) =>
        new(BuildKey(name, country, lat, lng),
            string.IsNullOrWhiteSpace(nameEn) ? name : nameEn!,
            string.IsNullOrWhiteSpace(nameEs) ? name : nameEs!,
            country.ToUpperInvariant(),
            lat,
            lng,
            "UTC");

    /// <summary>
    /// Deterministic, collision-safe natural key: <c>{name-slug≤24}-{country}-{lat3}-{lng3}</c>,
    /// coordinates rounded to 3 decimals (~110 m). The same place always yields the same key
    /// (idempotent import); places &gt;110 m apart yield different keys.
    /// </summary>
    public static string BuildKey(string name, string country, decimal lat, decimal lng)
    {
        var slug = Slugify(name);
        if (slug.Length > 24) slug = slug[..24].TrimEnd('-');
        var latR = Math.Round(lat, 3).ToString(CultureInfo.InvariantCulture);
        var lngR = Math.Round(lng, 3).ToString(CultureInfo.InvariantCulture);
        var key = $"{slug}-{country.ToLowerInvariant()}-{latR}-{lngR}";
        return key.Length > 50 ? key[..50] : key;
    }

    private static string Slugify(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        var lastWasHyphen = false;
        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark) continue;
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
                lastWasHyphen = false;
            }
            else if (!lastWasHyphen)
            {
                sb.Append('-');
                lastWasHyphen = true;
            }
        }
        return sb.ToString().Trim('-');
    }
}
