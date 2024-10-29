namespace BeerRateApi.DTOs
{
    public class AddBeerResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
    }
}
