using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Describes a beer entity, including properties like name, producer, alcohol amount, and other beer-specific details.
    /// </summary>
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
