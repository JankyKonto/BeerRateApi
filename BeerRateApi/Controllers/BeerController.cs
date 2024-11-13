using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
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

        [HttpPost("addbeer")]
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
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getbeers")]
        public async Task<IActionResult> GetBeers ()
        {
            try
            {
                var getBeersResult = await _beerService.GetBeers();
                return Ok(new { Beers = getBeersResult });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpGet("getbeer")]
        public async Task<IActionResult> GetBeer(int id)
        {
            try
            {
                var getBeerResult = await _beerService.GetBeer(id);
                return Ok(getBeerResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("filterbeers")]
        public async Task<IActionResult> FilterBeers(
            string name,
            string producer,
            string kind,
            string originCountry,
            decimal? minAlcoholAmount,
            decimal? maxAlcoholAmount,
            int? minIbu,
            int? maxIbu)
        {
            try
            {
                var getBeersResult = await _beerService.FilterBeers(name, producer, kind, originCountry, minAlcoholAmount, maxAlcoholAmount, minIbu, maxIbu);
                return Ok(getBeersResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message});
            }
        }
    }
}
