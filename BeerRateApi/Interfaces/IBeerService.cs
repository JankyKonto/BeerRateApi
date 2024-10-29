using BeerRateApi.DTOs;

namespace BeerRateApi.Interfaces
{
    public interface IBeerService
    {
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        //Task<AddBeerResult> UpdateBeer(AddBeerDTO addBeerDTO);
    }
}
