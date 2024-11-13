using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Interfaces
{
    public interface IBeerService
    {
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        Task<IEnumerable<BeerDTO>> GetBeers ();
        Task<BeerDTO> GetBeer (int id);
        Task<GetBeersResult> FilterAndSortBeers(FilterAndSortBeersDTO dto);
    }
}
