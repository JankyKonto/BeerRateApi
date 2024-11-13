using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerRateApi.Services
{
    public class BeerService : BaseService, IBeerService
    {
        public BeerService(AppDbContext dbContext, ILogger logger, IMapper mapper) : base(dbContext, logger, mapper)
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

                return new AddBeerResult { Name = addBeerDTO.Name };
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
                if (beer != null)
                    return new GetBeerResult { Name = beer.Name, Producer = beer.Producer, Kind = beer.Kind, OriginCountry = beer.OriginCountry, AlcoholAmount = beer.AlcoholAmount, Ibu = beer.Ibu, BeerImage = beer.BeerImage, BeerImageId = beer.BeerImageId };
                else
                    throw new InvalidOperationException($"Beer with id '{id}' not found.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<BeerListElementDTO>> GetBeers()
        {
            try
            {
                var beers = await DbContext.Beers.ToListAsync();
                return Mapper.Map<IEnumerable<BeerListElementDTO>>(beers);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<GetBeersResult> FilterAndSortBeers(FilterAndSortBeersDTO dto)
        {
            try
            {
                var query = DbContext.Beers.AsQueryable();

                if (!string.IsNullOrEmpty(dto.Name))
                {
                    query = query.Where(b => b.Name.Contains(dto.Name));
                }

                if (!string.IsNullOrEmpty(dto.Producer))
                {
                    query = query.Where(b => b.Producer.Contains(dto.Producer));
                }

                if (!string.IsNullOrEmpty(dto.Kind))
                {
                    query = query.Where(b => b.Kind.Contains(dto.Kind));
                }

                if (!string.IsNullOrEmpty(dto.OriginCountry))
                {
                    query = query.Where(b => b.OriginCountry.Contains(dto.OriginCountry));
                }

                if (dto.MinAlcoholAmount.HasValue)
                {
                    query = query.Where(b => b.AlcoholAmount >= dto.MinAlcoholAmount.Value);
                }

                if (dto.MaxAlcoholAmount.HasValue)
                {
                    query = query.Where(b => b.AlcoholAmount <= dto.MaxAlcoholAmount.Value);
                }

                if (dto.MinIbu.HasValue)
                {
                    query = query.Where(b => b.Ibu >= dto.MinIbu.Value);
                }

                if (dto.MaxIbu.HasValue)
                {
                    query = query.Where(b => b.Ibu <= dto.MaxIbu.Value);
                }

                if (dto.isAscending == true)
                {
                    query = query.OrderBy(b => dto.SortType.ToString());
                }
                else
                {
                    query = query.OrderByDescending(b => dto.SortType.ToString());
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
