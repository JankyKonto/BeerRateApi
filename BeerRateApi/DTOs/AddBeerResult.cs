namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Encapsulates the result of adding a beer, such as name and producer.
    /// </summary>
    public class AddBeerResult
    {
        public string Name { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
    }
}

