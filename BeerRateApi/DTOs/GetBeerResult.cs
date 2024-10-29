using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    public class GetBeerResult
    {
        public string Name { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string OriginCountry { get; set; } = string.Empty;
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public int BeerImageId { get; set; }
        public virtual BeerImage BeerImage { get; set; }
    }
}
