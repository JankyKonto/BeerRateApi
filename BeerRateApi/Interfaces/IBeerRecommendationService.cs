using BeerRateApi.DTOs;

namespace BeerRateApi.Interfaces
{
    public interface IBeerRecommendationService
    {
        Task<IEnumerable<BeerDTO>> RecommendSimilarBeers(int beerId, int numberOfRecommendations);
    }
}
