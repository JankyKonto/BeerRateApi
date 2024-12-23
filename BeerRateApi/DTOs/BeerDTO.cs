﻿using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    public class BeerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Producer { get; set; }
        public int Kind { get; set; }
        public string OriginCountry { get; set; }
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public double TasteAverage { get; set; }
        public double AromaAverage { get; set; }
        public double FoamAverage { get; set; }
        public double ColorAverage { get; set; }
    }
}
