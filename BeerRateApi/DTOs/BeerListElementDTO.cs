using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    public class BeerListElementDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Producer { get; set; }
        public string Kind { get; set; }
        public string OriginCountry { get; set; }
        public decimal AlcoholAmount { get; set; }
        public int Ibu { get; set; }
        public byte[] Image { get; set; }
    }
}
