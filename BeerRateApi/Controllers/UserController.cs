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
        public async Task<IActionResult> RemindPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email address is required");

            try
            {
                // Tworzenie wiadomości e-mail
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Milosz", "beerratemail@gmail.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Beer rate wiadomosc";

                message.Body = new TextPart("plain")
                {
                    Text = "Testujemy piwerko mordo"
                };

                // Wysyłanie wiadomości przy użyciu MailKit
                using (var client = new SmtpClient())
                {
                    // Łączymy się z serwerem Gmail SMTP
                    await client.ConnectAsync("smtp.gmail.com", 465, true);

                    // Uwierzytelnianie za pomocą konta Gmail
                    await client.AuthenticateAsync("beerratemail@gmail.com", "jankyhaslo12");

                    // Wysyłanie wiadomości
                    await client.SendAsync(message);

                    // Rozłączamy się z serwerem
                    await client.DisconnectAsync(true);
                }

                return Ok("Email sent successfully");
            }
            catch (SmtpCommandException ex)
            {
                return StatusCode(500, $"SMTP Command error: {ex.Message}");
            }
            catch (SmtpProtocolException ex)
            {
                return StatusCode(500, $"SMTP Protocol error: {ex.Message}");
            }
        }


    }
}
