using BeerRateApi.DTOs;

namespace BeerRateApi.Interfaces
{
    public interface IBeerRecommendationService
    {
        /// <summary>
        /// Recommends similar beers based on clustering.
        /// </summary>
        /// <param name="beerId">The ID of the beer to find similar beers for.</param>
        /// <param name="numberOfRecommendations">The number of recommendations to return. Default is 3.</param>
        /// <returns>A collection of recommended beers as DTOs.</returns>
        /// <exception cref="ArgumentException">Thrown when the beer with the given ID is not found.</exception>
        Task<IEnumerable<BeerDTO>> RecommendSimilarBeers(int beerId, int numberOfRecommendations);
    }
}
