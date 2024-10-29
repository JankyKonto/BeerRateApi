﻿using BeerRateApi.DTOs;
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

        public async Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO)
        {
            try
            {
                if (await DbContext.Beers.AnyAsync(beer => beer.Name == addBeerDTO.Name))
                {
                    throw new InvalidOperationException($"Beer with name '{addBeerDTO.Name}' already exists.");
                }

                var beer = new Beer { Name = addBeerDTO.Name, Producer = addBeerDTO.Producer, Kind = addBeerDTO.Kind, OriginCountry = addBeerDTO.OriginCountry, AlcoholAmount = addBeerDTO.AlcoholAmount, Ibu = addBeerDTO.Ibu, BeerImage = addBeerDTO.BeerImage };
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
    }
}
