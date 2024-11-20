using BeerRateApi.DTOs;

namespace BeerRateApi.Interfaces
{
    public interface IBeerReviewService
    {
        Task<AddBeerReviewResult> AddBeerReview(AddBeerReviewDTO AddBeerReviewDTO);
        Task<GetBeerReviewResult> GetBeerReview(int id);
        Task<int> GetReviewsCounter(int id);
        Task<IQueryable<GetBeerReviewResult>> GetBeerReviews(int beerId, int startIndex, int endIndex);
        Task<IQueryable<GetBeerReviewResult>> GetBeerReviewsPage(int beerId, int page);
        Task<int> GetBeerReviewPagesAmount(int beerId);
    }
}
