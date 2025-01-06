using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerRateApi.Services
{
    /// <summary>
    /// Service for managing beer reviews.
    /// </summary>
    public class BeerReviewService : BaseService, IBeerReviewService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeerReviewService"/> class.
        /// </summary>
        /// <param name="dbContext">Database context for accessing data.</param>
        /// <param name="logger">Logger for logging errors and information.</param>
        /// <param name="mapper">Mapper for object-to-object mapping.</param>
        public BeerReviewService(AppDbContext dbContext, ILogger logger, IMapper mapper)
            : base(dbContext, logger, mapper) { }


        /// <summary>
        /// Number of reviews displayed per page.
        /// </summary>
        private const int reviewsPerPage = 10;

        /// <summary>
        /// Adds a new beer review to the database.
        /// </summary>
        /// <param name="addBeerReviewDTO">Data transfer object containing the details of the beer review to add.</param>
        /// <returns>A result containing the username of the user who added the review.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the user has already reviewed the beer.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        public async Task<AddBeerReviewResult> AddBeerReview(AddBeerReviewDTO addBeerReviewDTO)
        {
            try
            {
                if (await DbContext.Reviews.AnyAsync(review => review.UserId == addBeerReviewDTO.UserId && review.BeerId == addBeerReviewDTO.BeerId))
                {
                    throw new InvalidOperationException($"This user has been already rated this beer");
                }

                var review = new Review()
                {
                    Text = addBeerReviewDTO.Text,
                    TasteRate = addBeerReviewDTO.TasteRate,
                    AromaRate = addBeerReviewDTO.AromaRate,
                    FoamRate = addBeerReviewDTO.FoamRate,
                    ColorRate = addBeerReviewDTO.ColorRate,
                    BeerId = addBeerReviewDTO.BeerId,
                    UserId = addBeerReviewDTO.UserId,
                    CreatedAt = DateTime.Now,
                };
                DbContext.Reviews.Add(review);
                await DbContext.SaveChangesAsync();
                var user = await DbContext.Users.FindAsync(addBeerReviewDTO.UserId);
                string username = user.Username;
                return new AddBeerReviewResult()
                {
                    Username = username
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific beer review by its ID.
        /// </summary>
        /// <param name="id">The ID of the beer review to retrieve.</param>
        /// <returns>A result containing the details of the beer review.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        public async Task<GetBeerReviewResult> GetBeerReview(int id)
        {
            try
            {
                var review = await DbContext.Reviews.FindAsync(id);
                return new GetBeerReviewResult()
                {
                    Id = review.Id,
                    Text = review.Text,
                    TasteRate = review.TasteRate,
                    AromaRate = review.AromaRate,
                    FoamRate = review.FoamRate,
                    ColorRate = review.ColorRate,
                    BeerId = review.BeerId,
                    UserId = review.UserId,
                    UserName = review.User.Username
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        /// <summary>
        /// Gets the total number of reviews for a specific beer.
        /// </summary>
        /// <param name="id">The ID of the beer.</param>
        /// <returns>The total number of reviews for the beer.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the beer is not found.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        public async Task<int> GetReviewsCounter(int id)
        {
            try
            {
                var beer = await DbContext.Beers.FindAsync(id);

                if(beer == null)
                {
                    throw new UnauthorizedAccessException("Beer not found");
                }

                return beer.Reviews.Count;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a list of reviews for a specific beer within a specified range.
        /// </summary>
        /// <param name="beerId">The ID of the beer.</param>
        /// <param name="startIndex">The start index of the reviews to retrieve.</param>
        /// <param name="endIndex">The end index of the reviews to retrieve.</param>
        /// <returns>A list of beer reviews within the specified range.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        public async Task<IEnumerable<GetBeerReviewResult>> GetBeerReviews(int beerId, int startIndex, int endIndex)
        {
            try
            {
                var reviews =
                    DbContext.Reviews
                    .Where(review => review.BeerId == beerId)
                    .OrderByDescending(review => review.CreatedAt)
                    .Skip(startIndex)
                    .Take(endIndex - startIndex)
                    .Select
                    (
                        review => new GetBeerReviewResult()
                        {
                            Id = review.Id,
                            Text = review.Text,
                            TasteRate = review.TasteRate,
                            AromaRate = review.AromaRate,
                            FoamRate = review.FoamRate,
                            ColorRate = review.ColorRate,
                            BeerId = review.BeerId,
                            UserId = review.UserId,
                            UserName = review.User.Username,
                            CreatedAt = review.CreatedAt
                        }
                    );
                return await reviews.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific page of reviews for a given beer.
        /// </summary>
        /// <param name="beerId">The ID of the beer.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <returns>A list of reviews for the specified page.</returns>
        /// <exception cref="ArgumentException">Thrown when the page number is less than 1.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during execution.</exception>
        public async Task<IEnumerable<GetBeerReviewResult>> GetBeerReviewsPage(int beerId, int page)
        {
            if (page < 1)
            {
                throw new ArgumentException("Wrong page number");
            }

            return await GetBeerReviews(beerId, (page - 1) * reviewsPerPage, reviewsPerPage * page);
        }
        /// <summary>
        /// Calculates the total number of pages of reviews for a specific beer.
        /// </summary>
        /// <param name="beerId">The ID of the beer.</param>
        /// <returns>The total number of pages of reviews.</returns>
        public async Task<int> GetBeerReviewPagesAmount(int beerId)
        {
            var counter = await GetReviewsCounter(beerId);
            return counter % reviewsPerPage == 0 ? counter / reviewsPerPage : counter / reviewsPerPage + 1;
        }
    }
}