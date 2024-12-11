using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BeerRateApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private readonly IBeerService _beerService;
        private readonly IBeerRecommendationService _beerRecommendationService;

        public BeerController(IBeerService beerService, IBeerRecommendationService beerRecommendationService) 
        {
            _beerService = beerService;
            _beerRecommendationService = beerRecommendationService;
        }

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
        [AllowAnonymous]
        [HttpGet("similar-beers/{beerId}")]
        public async Task<IActionResult> GetSimilarBeers(int beerId)
        {
            try
            {
                var recomendations = await _beerRecommendationService.RecommendSimilarBeers(beerId, 3);
                return Ok(recomendations);
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
