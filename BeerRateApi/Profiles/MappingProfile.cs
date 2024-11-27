using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Beer, BeerDTO>();
        }
    }
}
