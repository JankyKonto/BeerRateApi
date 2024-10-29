using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeerRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private readonly IBeerService _beerService;

        public BeerController(IBeerService beerService) { _beerService = beerService; }

        [HttpPost("addbeer")]
        public async Task<IActionResult> AddBeer (AddBeerDTO addBeerDTO)
        {
            try
            {
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
    }
}
