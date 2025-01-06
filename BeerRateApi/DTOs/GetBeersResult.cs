using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Represents a list or collection of beers retrieved from the system, often including pagination or metadata.
    /// </summary>
    public class GetBeersResult
    {
        public IEnumerable<BeerDTO>? Beers;
    }
}
