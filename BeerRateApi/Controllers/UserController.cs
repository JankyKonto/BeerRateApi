using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Http;
using System.Security.Claims;

namespace BeerRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService) { _userService = userService; }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var registerResult = await _userService.RegisterUser(registerDTO);
                return Ok(registerResult);
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh ()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (refreshToken == null)
                {
                    return Unauthorized(new { Message = "RefreshToken is null" });
                }

                var expiredToken = HttpContext.Request.Cookies["jwtToken"];
                if (expiredToken == null)
                {
                    return Unauthorized(new { Message = "ExpiredToken is null" });
                }

                var refreshResult = await _userService.Refresh(expiredToken, refreshToken);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(1)
                };

                Response.Cookies.Append("jwtToken", refreshResult.JwtToken, cookieOptions);

                return Ok(new { refreshResult.Id, refreshResult.Username, refreshResult.Email });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpDelete("revoke")]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { Message = "Invalid user identifier." });
                }

                await _userService.Revoke(userId);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                Response.Cookies.Delete("refreshToken", cookieOptions);
                Response.Cookies.Delete("jwtToken", cookieOptions);

                return Ok(new {});

            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new {ex.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { ex.Message });
            }

        }
        [HttpPost("remind-password")]
        public async Task<IActionResult> RemindPassword()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid user identifier." });
            }

            try
            {
                await _userService.RemindPassword(userId);
                return Ok();
            }
            catch (ArgumentNullException ex)
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
