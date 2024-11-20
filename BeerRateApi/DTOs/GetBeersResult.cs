using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    public class GetBeersResult
    {
        public IEnumerable<BeerDTO>? Beers;
    }
}
