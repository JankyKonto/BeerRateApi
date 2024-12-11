using BeerRateApi.DTOs;

namespace BeerRateApi.Interfaces
{
    public interface IBeerRecommendationService
    {
        IEnumerable<BeerDTO> RecommendSimilarBeers(int beerId, int numberOfRecommendations);
    }
}
