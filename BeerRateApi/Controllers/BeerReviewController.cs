using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

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
