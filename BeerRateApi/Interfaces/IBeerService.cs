using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Interfaces
{
    public interface IBeerService
    {
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        Task<BeerDTO> GetBeer (int id);
        Task<IEnumerable<BeerDTO>> FilterAndSortBeers(FilterAndSortBeersDTO dto, int startIndex, int endIndex);
        Task<int> GetBeersCounter();
        Task<IEnumerable<BeerDTO>> GetBeersPage(int page, FilterAndSortBeersDTO dto);
        Task<int> GetBeersPagesAmount();
        Task<byte[]> GetBeerImage(int id);
    }
}
