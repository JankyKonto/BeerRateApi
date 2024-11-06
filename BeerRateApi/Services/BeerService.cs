using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerRateApi.Services
{
    public class BeerService : BaseService, IBeerService
    {
        public BeerService(AppDbContext dbContext, ILogger logger) : base(dbContext, logger)
        {

        }

        private static async Task<BeerImage?> ConvertIFormFileToBeerImage(IFormFile formFile, string caption = "")
        {
            if (formFile == null || formFile.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);

                return new BeerImage
                {
                    Data = memoryStream.ToArray(),
                    ContentType = formFile.ContentType,
                    Caption = caption
                };
            }
        }

        public async Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO)
        {
            try
            {
                if (await DbContext.Beers.AnyAsync(beer => beer.Name == addBeerDTO.Name))
                {
                    throw new InvalidOperationException($"Beer with name '{addBeerDTO.Name}' already exists.");
                }

                var beer = new Beer { Name = addBeerDTO.Name, Producer = addBeerDTO.Producer, Kind = addBeerDTO.Kind, OriginCountry = addBeerDTO.OriginCountry, AlcoholAmount = addBeerDTO.AlcoholAmount, Ibu = addBeerDTO.Ibu, BeerImage = await ConvertIFormFileToBeerImage(addBeerDTO.BeerImage) };
                DbContext.Beers.Add(beer);
                await DbContext.SaveChangesAsync();

                return new AddBeerResult { Id = addBeerDTO.Id, Name = addBeerDTO.Name };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        public async Task<GetBeerResult> GetBeer(int id)
        {
            try
            {
                var beer = await DbContext.Beers.FindAsync(id);
                return new GetBeerResult { Name = beer.Name, Producer = beer.Producer, Kind = beer.Kind, OriginCountry = beer.OriginCountry, AlcoholAmount = beer.AlcoholAmount, Ibu = beer.Ibu, BeerImage = beer.BeerImage, BeerImageId = beer.BeerImageId };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<GetBeersResult> GetBeers()
        {
            try
            {
                var beers = await DbContext.Beers.ToListAsync();
                return new GetBeersResult { Beers = beers };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }


    }
}
