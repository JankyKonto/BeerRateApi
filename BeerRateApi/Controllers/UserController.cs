using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BeerRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

    }
}
