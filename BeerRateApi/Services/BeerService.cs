using System.Diagnostics.Metrics;
using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BeerRateApi.Services
{
    public class BeerService : BaseService, IBeerService
    {
        public BeerService(AppDbContext dbContext, ILogger logger, IMapper mapper) : base(dbContext, logger, mapper)
        {

        }
        private const int beersPerPage = 10;
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
                    FileType = formFile.FileName,
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

                if (addBeerDTO.Ibu < 0 || addBeerDTO.AlcoholAmount < 0)
                {
                    throw new InvalidOperationException($"Value cannot be negative");
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

        public async Task<BeerDTO> GetBeer(int id)
        {
            try
            {
                var beer = await DbContext.Beers.FirstOrDefaultAsync(b => b.Id == id && b.IsRemoved == false);
                if (beer != null)
                    return Mapper.Map<BeerDTO>(beer);
                else
                    throw new InvalidOperationException($"Beer with id '{id}' not found.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<PagesWithBeersDTO> FilterAndSortBeers(FilterAndSortBeersDTO dto, int page)
        {
            var startIndex = (page - 1) * beersPerPage;
            var endIndex = beersPerPage * page;

            try
            {
                var query = DbContext.Beers.Where(b => b.IsRemoved == false && b.IsConfirmed == true).AsQueryable();

                if (!string.IsNullOrEmpty(dto.Name))
                {
                    query = query.Where(b => b.Name.Contains(dto.Name));
                }

                if (!string.IsNullOrEmpty(dto.Producer))
                {
                    query = query.Where(b => b.Producer.Contains(dto.Producer));
                }

                if (dto.Kind != 0 && dto.Kind != null)
                {
                    query = query.Where(b => b.Kind == dto.Kind);
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

                var sortType = (int)dto.SortType;
                if (dto.isAscending == true)
                {
                    switch (sortType)
                    {
                        case 0:
                            query = query.OrderBy(b => b.Name);
                            break;
                        case 1:
                            query = query.OrderBy(b => b.AlcoholAmount);
                            break;
                        case 2:
                            query = query.OrderBy(b => b.Ibu);
                            break;
                        default:
                            query = query.OrderBy(b => b.Name);
                            break;
                    }
                }
                else
                {
                    switch (sortType)
                    {
                        case 0:
                            query = query.OrderByDescending(b => b.Name);
                            break;
                        case 1:
                            query = query.OrderByDescending(b => b.AlcoholAmount);
                            break;
                        case 2:
                            query = query.OrderByDescending(b => b.Ibu);
                            break;
                        default:
                            query = query.OrderByDescending(b => b.Name);
                            break;
                    }
                }
                var beersCount = await query.CountAsync();
                var pages = beersCount % beersPerPage == 0 ? beersCount / beersPerPage : beersCount / beersPerPage + 1;
                query = query.Skip(startIndex).Take(endIndex - startIndex);

                var beers = await query.ToListAsync();

                if(page > pages && pages != 0)
                {
                    throw new ArgumentException("Wrong page number");
                }

                var pageWithBeers = new PagesWithBeersDTO();

                pageWithBeers.Pages = pages;
                pageWithBeers.Beers = Mapper.Map<IEnumerable<BeerDTO>>(beers);

                return pageWithBeers;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<PagesWithBeersDTO> GetBeersPage(int page, FilterAndSortBeersDTO dto)
        {
            int pagesAmount = await GetBeersPagesAmount();
            if (page == 0)
            {
                page = 1;
            }
            else if (page < 1 || page > pagesAmount)
            {
                throw new ArgumentException("Wrong page number");
            }

            return await FilterAndSortBeers(dto, page);
        }

        public async Task<int> GetBeersPagesAmount()
        {
            var counter = await GetBeersCounter();
            return counter % beersPerPage == 0 ? counter / beersPerPage : counter / beersPerPage + 1;
        }

        public async Task<int> GetBeersCounter()
        {
            try
            {
                return await DbContext.Beers.CountAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<byte[]> GetBeerImage(int id)
        {
            var beer = await DbContext.Beers.FindAsync(id);

            if (beer == null)
            {
                throw new InvalidOperationException($"Beer with id '{id}' not found.");
            }
            return beer.BeerImage.Data;

        }

        public async Task ConfirmBeer(int beerId, int userId)
        {
            var user = await DbContext.Users.FindAsync(userId);
            if (user != null && user.UserType == Enums.UserType.Admin)
            {
                var beer = await DbContext.Beers.FindAsync(beerId);
                if (beer != null)
                {
                    beer.IsConfirmed = true;
                    DbContext.Beers.Update(beer);
                    await DbContext.SaveChangesAsync();
                }
                else
                    throw new InvalidOperationException($"Beer with id '{beerId}' not found.");
            }
            else
            {
                throw new UnauthorizedAccessException($"User is not an Admin!");
            }


        }

        public async Task DeleteBeer(int beerId, int userId)
        {
            var user = await DbContext.Users.FindAsync(userId);
            if (user != null && user.UserType == Enums.UserType.Admin)
            {
                var beer = await DbContext.Beers.FindAsync(beerId);
                if (beer != null)
                {
                    beer.IsRemoved = true;
                    DbContext.Beers.Update(beer);
                    await DbContext.SaveChangesAsync();
                }
                else
                    throw new InvalidOperationException($"Beer with id '{beerId}' not found.");
            }
            else
            {
                throw new UnauthorizedAccessException($"User is not an Admin!");
            }
        }

        public async Task<PagesWithBeersDTO> GetUnconfirmedBeers(int page)
        {
            var startIndex = (page - 1) * beersPerPage;
            var endIndex = beersPerPage * page;
            var query = DbContext.Beers.Where(b => b.IsConfirmed == false && b.IsRemoved == false).AsQueryable();
            var beersCount = await query.CountAsync();
            var pages = beersCount % beersPerPage == 0 ? beersCount / beersPerPage : beersCount / beersPerPage + 1;
            query = query.Skip(startIndex).Take(endIndex - startIndex);

            var beers = await query.ToListAsync();

            if (page > pages)
            {
                throw new ArgumentException("Wrong page number");
            }

            var pageWithBeers = new PagesWithBeersDTO();

            pageWithBeers.Pages = pages;
            pageWithBeers.Beers = Mapper.Map<IEnumerable<BeerDTO>>(beers);

            return pageWithBeers;
        }
    }
}
