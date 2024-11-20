using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BeerRateApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeerReviewController : Controller
    {
        public readonly IBeerReviewService _beerReviewService;
        private const int reviewsPerPage = 20;
        public BeerReviewController(IBeerReviewService beerReviewService)
        {
            _beerReviewService = beerReviewService;
        }

        [HttpPost("add-beer-review")]
        public async Task<IActionResult> AddBeerReview(AddBeerReviewDTO addBeerReviewDTO)
        {
            try
            {
                var addBeerReviewResult = await _beerReviewService.AddBeerReview(addBeerReviewDTO);
                return Ok(addBeerReviewResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("get-beer-review/{id}")]
        public async Task<IActionResult> GetBeerReview(int id)
        {
            try
            {
                var getBeerReviewResult = await _beerReviewService.GetBeerReview(id);
                return Ok(getBeerReviewResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("get-beer-reviews-counter/{id}")]
        public async Task<IActionResult> GetBeerReviewsCounter(int id)
        {
            try
            {
                var counter = await _beerReviewService.GetReviewsCounter(id);
                return Ok(new { Counter = counter } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });

            }
        }

        [AllowAnonymous]
        [HttpGet("get-beer-reviews/{id}")]
        public async Task<IActionResult> GetBeerReviews(int id, int startIndex, int endIndex)
        {
            try
            {
                var reviews = await _beerReviewService.GetBeerReviews(id, startIndex, endIndex); 
                return Ok(new { Reviews = reviews });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("getbeerreviewspage/{id}")]
        public async Task<IActionResult> GetBeerReviewsPage(int id, int page)
        {

            try
            {
                var reviews = await _beerReviewService.GetBeerReviews(id, (page-1)*reviewsPerPage, reviewsPerPage * page-1);
                return Ok(new { Reviews = reviews });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
        [HttpGet("getbeerreviewpagesamount/{id}")]
        public async Task<IActionResult> GetBeerReviewPagesAmount(int id, int page)
        {

            try
            {
                var counter = await _beerReviewService.GetReviewsCounter(id);
                int result = counter % reviewsPerPage == 0? counter /reviewsPerPage : counter / reviewsPerPage+1;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
