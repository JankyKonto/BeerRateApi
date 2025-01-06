using BeerRateApi.DTOs;
using BeerRateApi.Models;

namespace BeerRateApi.Interfaces
{
    /// <summary>
    /// An interface for managing beers
    /// </summary>
    public interface IBeerService
    {
        /// <summary>
        /// Adds a new beer to the database.
        /// </summary>
        /// <param name="addBeerDTO">The data transfer object containing beer details.</param>
        /// <returns>The result of adding a beer.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a beer with the same name already exists.</exception>
        Task<AddBeerResult> AddBeer(AddBeerDTO addBeerDTO);
        /// <summary>
        /// Retrieves a beer by its ID.
        /// </summary>
        /// <param name="id">The ID of the beer to retrieve.</param>
        /// <returns>A <see cref="BeerDTO"/> object representing the beer.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the beer is not found.</exception>
        Task<BeerDTO> GetBeer (int id);
        /// <summary>
        /// Filters and sorts beers based on the provided criteria.
        /// </summary>
        /// <param name="dto">The filter and sort criteria.</param>
        /// <param name="page">The page number.</param>
        /// <returns>A <see cref="PagesWithBeersDTO"/> containing the beers and pagination data.</returns>
        /// <exception cref="ArgumentException">Thrown when the page number is invalid.</exception>
        Task<PagesWithBeersDTO> FilterAndSortBeers(FilterAndSortBeersDTO dto, int page);
        /// <summary>
        /// Retrieves a page of unconfirmed beers.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <returns>A <see cref="PagesWithBeersDTO"/> containing unconfirmed beers and pagination details.</returns>
        /// <exception cref="ArgumentException">Thrown when the page number is invalid.</exception>
        Task<PagesWithBeersDTO> GetUnconfirmedBeers(int page);
        /// <summary>
        /// Confirms a beer by setting its <c>IsConfirmed</c> property to true.
        /// </summary>
        /// <param name="beerId">The ID of the beer to confirm.</param>
        /// <param name="userId">The ID of the user performing the confirmation.</param>
        /// <exception cref="InvalidOperationException">Thrown when the beer is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not an admin.</exception>
        Task ConfirmBeer(int beerId, int userId);
        /// <summary>
        /// Marks a beer as removed by setting its <c>IsRemoved</c> property to true.
        /// </summary>
        /// <param name="beerId">The ID of the beer to delete.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <exception cref="InvalidOperationException">Thrown when the beer is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not an admin.</exception>
        Task DeleteBeer(int beerId, int userId);
        /// <summary>
        /// Retrieves the total number of beers in the database.
        /// </summary>
        /// <returns>The total number of beers.</returns>
        /// <exception cref="Exception">Thrown when there is an error querying the database.</exception>
        Task<int> GetBeersCounter();
        /// <summary>
        /// Retrieves a specific page of beers based on the provided page number and filter criteria.
        /// </summary>
        /// <param name="page">The page number to retrieve. Defaults to 1 if 0 is provided.</param>
        /// <param name="dto">The filter and sort criteria.</param>
        /// <returns>A <see cref="PagesWithBeersDTO"/> containing the beers and pagination details.</returns>
        /// <exception cref="ArgumentException">Thrown when the page number is invalid.</exception>
        Task<PagesWithBeersDTO> GetBeersPage(int page, FilterAndSortBeersDTO dto);
        /// <summary>
        /// Retrieves the total number of pages available for beers.
        /// </summary>
        /// <returns>The total number of pages based on the current beer count.</returns>
        Task<int> GetBeersPagesAmount();
        /// <summary>
        /// Retrieves the image data for a specific beer by its ID.
        /// </summary>
        /// <param name="id">The ID of the beer whose image is being retrieved.</param>
        /// <returns>A byte array containing the beer image data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the beer is not found.</exception>
        Task<byte[]> GetBeerImage(int id);
    }
}
