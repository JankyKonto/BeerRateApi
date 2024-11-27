using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Interfaces
{
    public interface IBeerService
    {
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        Task<BeerDTO> GetBeer (int id);
        Task<PagesWithBeersDTO> FilterAndSortBeers(FilterAndSortBeersDTO dto, int page);
        Task<int> GetBeersCounter();
        Task<PagesWithBeersDTO> GetBeersPage(int page, FilterAndSortBeersDTO dto);
        Task<int> GetBeersPagesAmount();
        Task<byte[]> GetBeerImage(int id);
    }
}
