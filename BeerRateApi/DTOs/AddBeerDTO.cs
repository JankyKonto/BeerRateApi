using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    public class AddBeerDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string OriginCountry { get; set; } = string.Empty;
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public virtual IFormFile BeerImage { get; set; }
    }
}
