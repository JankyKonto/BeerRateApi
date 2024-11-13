using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Interfaces
{
    public interface IBeerService
    {
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        Task<IEnumerable<BeerListElementDTO>> GetBeers ();
        Task<GetBeerResult> GetBeer (int id);
        Task<GetBeersResult> FilterAndSortBeers(FilterAndSortBeersDTO dto);
    }
}
