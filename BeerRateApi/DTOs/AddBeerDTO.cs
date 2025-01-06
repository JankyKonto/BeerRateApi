using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Represents the data required to add a new beer to the system. Includes properties such as beer name, producer, kind and other relevant details.
    /// </summary>
    public class AddBeerDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public int Kind { get; set; }
        public string OriginCountry { get; set; } = string.Empty;
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public virtual IFormFile BeerImage { get; set; }
    }
}
