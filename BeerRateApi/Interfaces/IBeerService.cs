using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Interfaces
{
    public interface IBeerService
    {
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        //Task<AddBeerResult> UpdateBeer(AddBeerDTO addBeerDTO);
        Task<IEnumerable<BeerListElementDTO>> GetBeers ();
        Task<GetBeerResult> GetBeer (int id);
        Task<GetBeersResult> FilterBeers(string name = null, string producer = null, string kind = null, string originCountry = null, decimal? minAlcoholAmount = null, decimal? maxAlcoholAmount = null, int? minIbu = null, int? maxIbu = null);
    }
}
