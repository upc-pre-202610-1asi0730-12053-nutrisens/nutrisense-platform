using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

public static class CityAssembler
{
    public static CityResource ToResource(City city) =>
        new(city.Id, city.Key, city.NameEn, city.NameEs,
            city.Country, city.Lat, city.Lng, city.Timezone);

    public static CitySearchResultResource ToSearchResource(CitySearchResult result) =>
        new(result.Id, result.Key, result.NameEn, result.NameEs,
            result.Country, result.State, result.Lat, result.Lng);
}
