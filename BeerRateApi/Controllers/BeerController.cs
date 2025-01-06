using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BeerRateApi.Controllers
{
    /// <summary>
    /// Controller responsible for managing beer-related operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private readonly IBeerService _beerService;
        private readonly IBeerRecommendationService _beerRecommendationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeerController"/> class.
        /// </summary>
        /// <param name="beerService">Service for managing beers.</param>
        /// <param name="beerRecommendationService">Service for recommending beers.</param>
        public BeerController(IBeerService beerService, IBeerRecommendationService beerRecommendationService) 
        {
            _beerService = beerService;
            _beerRecommendationService = beerRecommendationService;
        }

        /// <summary>
        /// Adds a new beer to the database.
        /// </summary>
        /// <param name="addBeerDTO">Data Transfer Object containing beer details.</param>
        /// <returns>Returns the result of adding a beer or an error message.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddBeer ([FromForm] AddBeerDTO addBeerDTO)
        {
            try
            {
                if (addBeerDTO.BeerImage == null) 
                {
                    return BadRequest(new { Message = "No image uploaded." });
                }

                var addBeerResult = await _beerService.AddBeer(addBeerDTO);
                return Ok(addBeerResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a paginated list of beers based on filters and sorting options.
        /// </summary>
        /// <param name="page">Page number to retrieve.</param>
        /// <param name="dto">Filter and sorting options.</param>
        /// <returns>Returns a paginated list of beers.</returns>
        [AllowAnonymous]
        [HttpGet("beers")]
        public async Task<IActionResult> GetBeersPage (int page, [FromQuery] FilterAndSortBeersDTO dto)
        {
            try
            {
                var beers = await _beerService.GetBeersPage(page, dto);
                return Ok(beers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the total number of pages for beers.
        /// </summary>
        /// <returns>Returns the total pages amount.</returns>
        [AllowAnonymous]
        [HttpGet("pages-amount")]
        public async Task<IActionResult> GetBeersPagesAmount()
        {
            try
            {
                var pagesAmount = await _beerService.GetBeersPagesAmount();
                return Ok(new { pagesAmount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the total number of beers in the database.
        /// </summary>
        /// <returns>Returns the total number of beers.</returns>
        [AllowAnonymous]
        [HttpGet("beers-counter")]
        public async Task<IActionResult> GetBeersCounter()
        {
            try
            {
                var counter = await _beerService.GetBeersCounter();
                return Ok(new { Counter = counter });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific beer by its ID.
        /// </summary>
        /// <param name="id">The ID of the beer to retrieve.</param>
        /// <returns>Returns the beer details for the given ID.</returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBeer(int id)
        {
            try
            {
                var getBeerResult = await _beerService.GetBeer(id);
                return Ok(getBeerResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the image of a specific beer by its ID.
        /// </summary>
        /// <param name="id">The ID of the beer to retrieve the image for.</param>
        /// <returns>Returns the image of the beer in PNG format.</returns>
        [AllowAnonymous]
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetBeerImage(int id)
        {
            try
            {
                var getBeerImageResult = await _beerService.GetBeerImage(id);
                return File(getBeerImageResult,"image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Confirms a beer for a specific user.
        /// </summary>
        /// <param name="beerId">The ID of the beer to confirm.</param>
        /// <returns>Returns an empty response upon successful confirmation.</returns>
        [HttpPost("{beerId}/confirm")]
        public async Task<IActionResult> ConfirmBeer(int beerId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user identifier." });
            }
            try
            {
                await _beerService.ConfirmBeer(beerId, userId);
                return Ok( new { } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a beer from the database for a specific user.
        /// </summary>
        /// <param name="beerId">The ID of the beer to delete.</param>
        /// <returns>Returns an empty response upon successful deletion.</returns>
        [HttpDelete("{beerId}")]
        public async Task<IActionResult> DeleteBeer(int beerId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user identifier." });
            }
            try
            {
                await _beerService.DeleteBeer(beerId, userId);
                return Ok(new { });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves unconfirmed beers for a specific page.
        /// </summary>
        /// <param name="page">The page number to retrieve unconfirmed beers for.</param>
        /// <returns>Returns a list of unconfirmed beers for the requested page.</returns>
        [HttpGet("unconfirmed")]
        public async Task<IActionResult> GetUnconfirmedBeers (int page)
        {
            try
            {
                var beers = await _beerService.GetUnconfirmedBeers(page);
                return Ok(beers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a list of similar beers to the specified beer.
        /// </summary>
        /// <param name="beerId">The ID of the beer to find similar beers for.</param>
        /// <returns>Returns a list of recommended similar beers.</returns>
        [AllowAnonymous]
        [HttpGet("similar-beers/{beerId}")]
        public async Task<IActionResult> GetSimilarBeers(int beerId)
        {
            try
            {
                var recomendations = _beerRecommendationService.RecommendSimilarBeers(beerId, 3);
                return Ok(new { Beers = recomendations });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }

        }
    }
}
