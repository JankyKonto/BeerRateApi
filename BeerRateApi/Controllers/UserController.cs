using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeerRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService) { _userService = userService; }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var registerResult = await _userService.RegisterUser(registerDTO);
                return Ok(registerResult);
            }
            catch (Exception ex) 
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginDTO loginDTO)
        {
            try
            {
                var loginResult = await _userService.LoginUser(loginDTO);

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = loginResult.RefreshTokenExpiry
                };
                Response.Cookies.Append("refreshToken", loginResult.RefreshToken, refreshCookieOptions);


                var jwtCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(1)
                };

                Response.Cookies.Append("jwtToken", loginResult.JwtToken, jwtCookieOptions);

                return Ok(new { loginResult.Id, loginResult.Email, loginResult.Username});
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message});
            }
        }

    }
}
