namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Represents paginated results of beers, including total pages, and the list of beers on the page.
    /// </summary>
    public class PagesWithBeersDTO
    {
        public IEnumerable<BeerDTO> Beers { get; set; }
        public int Pages { get; set; }
    }
}
