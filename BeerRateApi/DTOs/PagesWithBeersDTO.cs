namespace BeerRateApi.DTOs
{
    public class PagesWithBeersDTO
    {
        public IEnumerable<BeerDTO> Beers { get; set; }
        public int Pages { get; set; }
    }
}
