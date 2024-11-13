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
        public BeerReviewController(IBeerReviewService beerReviewService)
        {
            _beerReviewService = beerReviewService;
        }

        [HttpPost("addbeerreview")]
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
        [HttpGet("getbeerreview/{id}")]
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
        [HttpGet("getbeerreviewscounter/{id}")]
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
        [HttpGet("getbeerreviews/{id}")]
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

    }
}
