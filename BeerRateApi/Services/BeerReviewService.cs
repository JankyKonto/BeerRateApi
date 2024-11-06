using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerRateApi.Services
{
    public class BeerReviewService :BaseService, IBeerReviewService
    {
        public BeerReviewService(AppDbContext dbContext, ILogger logger) 
            :base(dbContext, logger){}
        public async Task<AddBeerReviewResult> AddBeerReview(AddBeerReviewDTO AddBeerReviewDTO)
        {
            try
            {
                if (await DbContext.Reviews.AnyAsync(review => review.UserId == AddBeerReviewDTO.UserId))
                {
                    throw new InvalidOperationException($"This user has been already rated this beer");
                }

                var review = new Review()
                {
                   Text=AddBeerReviewDTO.Text,
                   TasteRate=AddBeerReviewDTO.TasteRate,
                   AromaRate = AddBeerReviewDTO.AromaRate,
                   FoamRate = AddBeerReviewDTO.FoamRate,
                   ColorRate = AddBeerReviewDTO.ColorRate,
                   BeerId = AddBeerReviewDTO.BeerId,
                   UserId = AddBeerReviewDTO.UserId,
                };
                DbContext.Reviews.Add(review);
                await DbContext.SaveChangesAsync();
                var user = await DbContext.Users.FindAsync(AddBeerReviewDTO.UserId);
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
                return new GetBeerReviewResult ()
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
                var counter = await DbContext.Reviews.CountAsync();
                return counter;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public async Task<IQueryable<GetBeerReviewResult>> GetBeerReviews(int beerId,int startIndex, int endIndex)
        {
            try
            {
                var reviews =
                    DbContext.Reviews
                    .Where(review=>review.BeerId == beerId)
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
                            UserName = review.User.Username
                        }
                    );
                return reviews;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
