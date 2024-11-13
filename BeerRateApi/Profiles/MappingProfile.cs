﻿using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Beer, BeerListElementDTO>()
                .ForMember(dest => dest.Image, x => x.MapFrom(src => src.BeerImage.Data));
        }
    }
}