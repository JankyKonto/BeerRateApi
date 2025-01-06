using BeerRateApi.DTOs;

namespace BeerRateApi.Interfaces
{
    
    public interface IBeerReviewService
    {
        /// <summary>
        /// Adds a new beer review to the database.
        /// </summary>
        /// <param name="addBeerReviewDTO">Data transfer object containing the details of the beer review to add.</param>
        /// <returns>A result containing the username of the user who added the review.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user has already reviewed the beer.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        Task<AddBeerReviewResult> AddBeerReview(AddBeerReviewDTO AddBeerReviewDTO);

        /// <summary>
        /// Retrieves a specific beer review by its ID.
        /// </summary>
        /// <param name="id">The ID of the beer review to retrieve.</param>
        /// <returns>A result containing the details of the beer review.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        Task<GetBeerReviewResult> GetBeerReview(int id);

        /// <summary>
        /// Gets the total number of reviews for a specific beer.
        /// </summary>
        /// <param name="id">The ID of the beer.</param>
        /// <returns>The total number of reviews for the beer.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the beer is not found.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        Task<int> GetReviewsCounter(int id);

        /// <summary>
        /// Retrieves a list of reviews for a specific beer within a specified range.
        /// </summary>
        /// <param name="beerId">The ID of the beer.</param>
        /// <param name="startIndex">The start index of the reviews to retrieve.</param>
        /// <param name="endIndex">The end index of the reviews to retrieve.</param>
        /// <returns>A list of beer reviews within the specified range.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        Task<IEnumerable<GetBeerReviewResult>> GetBeerReviews(int beerId, int startIndex, int endIndex);

        /// <summary>
        /// Retrieves a specific page of reviews for a given beer.
        /// </summary>
        /// <param name="beerId">The ID of the beer.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <returns>A list of reviews for the specified page.</returns>
        /// <exception cref="ArgumentException">Thrown when the page number is less than 1.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        Task<IEnumerable<GetBeerReviewResult>> GetBeerReviewsPage(int beerId, int page);

        /// <summary>
        /// Calculates the total number of pages of reviews for a specific beer.
        /// </summary>
        /// <param name="beerId">The ID of the beer.</param>
        /// <returns>The total number of pages of reviews.</returns>
        Task<int> GetBeerReviewPagesAmount(int beerId);
    }
}
