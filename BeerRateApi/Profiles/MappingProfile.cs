using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Profiles
{
    /// <summary>
    /// A profile for configuring object-to-object mappings with AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Constructor that defines the mappings for the application.
        /// </summary>
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
