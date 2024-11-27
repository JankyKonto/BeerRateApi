using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerRateApi.Services
{
    public class BeerReviewService : BaseService, IBeerReviewService
    {
        public BeerReviewService(AppDbContext dbContext, ILogger logger, IMapper mapper)
            : base(dbContext, logger, mapper) { }
        private const int reviewsPerPage = 10;
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
        //Method is getting id of beer as param
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
        public async Task<IEnumerable<GetBeerReviewResult>> GetBeerReviews(int beerId, int startIndex, int endIndex)
        {
            try
            {
                var reviews =
                    DbContext.Reviews
                    .Where(review => review.BeerId == beerId)
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

        public async Task<IEnumerable<GetBeerReviewResult>> GetBeerReviewsPage(int beerId, int page)
        {
            if (page < 1)
            {
                throw new ArgumentException("Wrong page number");
            }

            return await GetBeerReviews(beerId, (page - 1) * reviewsPerPage, reviewsPerPage * page);
        }

        public async Task<int> GetBeerReviewPagesAmount(int beerId)
        {
            var counter = await GetReviewsCounter(beerId);
            return counter % reviewsPerPage == 0 ? counter / reviewsPerPage : counter / reviewsPerPage + 1;
        }
    }
}