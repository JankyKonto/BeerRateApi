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
                    FileName = formFile.FileName,
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

        public async Task<GetBeersResult> FilterBeers(
            string name = null,
            string producer = null,
            string kind = null,
            string originCountry = null,
            decimal? minAlcoholAmount = null,
            decimal? maxAlcoholAmount = null,
            int? minIbu = null,
            int? maxIbu = null
            )
        {
            try
            {
                var query = DbContext.Beers.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(b => b.Name.Contains(name));
                }

                if (!string.IsNullOrEmpty(producer))
                {
                    query = query.Where(b => b.Producer.Contains(producer));
                }

                if (!string.IsNullOrEmpty(kind))
                {
                    query = query.Where(b => b.Kind.Contains(kind));
                }

                if (!string.IsNullOrEmpty(originCountry))
                {
                    query = query.Where(b => b.OriginCountry.Contains(originCountry));
                }

                if (minAlcoholAmount.HasValue)
                {
                    query = query.Where(b => b.AlcoholAmount >= minAlcoholAmount.Value);
                }

                if (maxAlcoholAmount.HasValue)
                {
                    query = query.Where(b => b.AlcoholAmount <= maxAlcoholAmount.Value);
                }

                if (minIbu.HasValue)
                {
                    query = query.Where(b => b.Ibu >= minIbu.Value);
                }

                if (maxIbu.HasValue)
                {
                    query = query.Where(b => b.Ibu <= maxIbu.Value);
                }

                var beers = await query.ToListAsync();
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
