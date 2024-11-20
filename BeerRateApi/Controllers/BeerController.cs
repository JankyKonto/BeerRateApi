using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeerRateApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private readonly IBeerService _beerService;

        public BeerController(IBeerService beerService) { _beerService = beerService; }

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

        /* [AllowAnonymous]
        [HttpGet("beers")]
        public async Task<IActionResult> GetBeers([FromQuery] FilterAndSortBeersDTO dto)
        {
            try
            {
                var getBeersResult = await _beerService.FilterAndSortBeers(dto);
                return Ok(getBeersResult.Beers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        } */

        [AllowAnonymous]
        [HttpGet("beers")]
        public async Task<IActionResult> GetBeersPage (int page, [FromQuery] FilterAndSortBeersDTO dto)
        {
            try
            {
                var beers = await _beerService.GetBeersPage(page, dto);
                return Ok(new { Beers = beers });
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
    }
}
