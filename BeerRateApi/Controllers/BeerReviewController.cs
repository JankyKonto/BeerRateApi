using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace BeerRateApi.Controllers
{
    /// <summary>
    /// Handles beer review operations such as adding a review, retrieving reviews, and getting review counters.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeerReviewController : Controller
    {
        /// <summary>
        /// The service for interacting with beer reviews.
        /// </summary>
        public readonly IBeerReviewService _beerReviewService;


        /// <summary>
        /// Initializes a new instance of the <see cref="BeerReviewController"/> class.
        /// </summary>
        /// <param name="beerReviewService">The service used for beer review operations.</param>
        public BeerReviewController(IBeerReviewService beerReviewService)
        {
            _beerReviewService = beerReviewService;
        }


        /// <summary>
        /// Adds a beer review.
        /// </summary>
        /// <param name="addBeerReviewDTO">The details of the beer review to add.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddBeerReview(AddBeerReviewDTO addBeerReviewDTO)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user identifier." });
            }
            addBeerReviewDTO.UserId = userId;
            try
            {
                var addBeerReviewResult = await _beerReviewService.AddBeerReview(addBeerReviewDTO);
                return Ok(addBeerReviewResult);
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
        /// Retrieves a specific beer review by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the beer review.</param>
        /// <returns>The requested beer review or an error message if not found.</returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBeerReview(int id)
        {
            try
            {
                var getBeerReviewResult = await _beerReviewService.GetBeerReview(id);
                return Ok(getBeerReviewResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the total number of reviews for a specific beer.
        /// </summary>
        /// <param name="beerId">The identifier of the beer.</param>
        /// <returns>The total number of reviews for the specified beer.</returns>
        [AllowAnonymous]
        [HttpGet("reviews-counter/{beerId}")]
        public async Task<IActionResult> GetBeerReviewsCounter(int beerId)
        {
            try
            {
                var counter = await _beerReviewService.GetReviewsCounter(beerId);
                return Ok(new { Counter = counter } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a paginated list of reviews for a specific beer.
        /// </summary>
        /// <param name="beerId">The identifier of the beer.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <returns>A paginated list of beer reviews.</returns>
        [AllowAnonymous]
        [HttpGet("reviews/{beerId}")]
        public async Task<IActionResult> GetBeerReviewsPage(int beerId, int page)
        {
            try
            {
                var reviews = await _beerReviewService.GetBeerReviewsPage(beerId, page);
                return Ok(new { Reviews = reviews });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the number of pages required to display all reviews for a specific beer.
        /// </summary>
        /// <param name="beerId">The identifier of the beer.</param>
        /// <returns>The number of pages needed to display all reviews for the beer.</returns>
        [AllowAnonymous]
        [HttpGet("pages-amount/{beerId}")]
        public async Task<IActionResult> GetBeerReviewPagesAmount(int beerId)
        {

            try
            {
                var pagesAmount = await _beerReviewService.GetBeerReviewPagesAmount(beerId);
                return Ok(new { pagesAmount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

    }
}
