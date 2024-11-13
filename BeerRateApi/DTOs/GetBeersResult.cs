using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    public class GetBeersResult
    {
        public IEnumerable<Beer>? Beers;
    }
}
