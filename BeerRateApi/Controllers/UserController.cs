using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MimeKit;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;

namespace BeerRateApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService) { _userService = userService; }

        [AllowAnonymous]
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
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        [AllowAnonymous]
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
                return Unauthorized(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh ()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (refreshToken == null)
                {
                    return Unauthorized(new ErrorMessageDTO { ErrorMessage = "RefreshToken is null" });
                }

                var expiredToken = HttpContext.Request.Cookies["jwtToken"];
                if (expiredToken == null)
                {
                    return Unauthorized(new ErrorMessageDTO { ErrorMessage = "ExpiredToken is null" });
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
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
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
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }

        }

        [AllowAnonymous]
        [HttpPost("remind-password-send-email")]
        public async Task<IActionResult> RemindPasswordSendEmail(RemindPasswordDTO remindPasswordDTO)
        {
            if (string.IsNullOrEmpty(remindPasswordDTO.Email))
            {
                return Unauthorized(new ErrorMessageDTO { ErrorMessage = "Email cannot be empty" });
            }
            try
            {
                await _userService.RemindPasswordSendEmail(remindPasswordDTO.Email);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMessageDTO { ErrorMessage = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpPost("realise-password-reminding")]
        public async Task<IActionResult> RemindPasswordRealization(string newPassword,string token)
        {
            try
            {
                await _userService.RealisePasswordReminding(newPassword, token);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (ArgumentException ex)
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
