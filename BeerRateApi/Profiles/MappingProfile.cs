using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Beer, BeerDTO>().ConstructUsing((src, res) =>
            {
                return new BeerDTO
                {
                    Id = src.Id,
                    Name = src.Name,
                    Producer = src.Producer,
                    Kind = src.Kind,
                    OriginCountry = src.OriginCountry,
                    AlcoholAmount = src.AlcoholAmount,
                    Ibu = src.Ibu,
                    TasteAverage = src.AverageTasteRate,
                    AromaAverage = src.AverageAromaRate,
                    FoamAverage = src.AverageFoamRate,
                    ColorAverage = src.AverageColorRate
                };
            });
        }
    }
}
