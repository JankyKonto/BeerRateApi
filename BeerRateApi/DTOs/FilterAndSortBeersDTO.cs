using BeerRateApi.Enums;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Provides criteria for filtering and sorting beers, such as kind, producer, or alphabetical order.
    /// </summary>
    public class FilterAndSortBeersDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Producer {  get; set; } = string.Empty;
        public int? Kind {  get; set; }
        public string OriginCountry {  get; set; } = string.Empty;
        public decimal? MaxAlcoholAmount { get; set; } = null;
        public decimal? MinAlcoholAmount { get; set; } = null;
        public int? MinIbu {  get; set; } = null;
        public int? MaxIbu { get; set; } = null;
        public SortType SortType { get; set; }
        public bool isAscending { get; set; } = true;
    }
}
