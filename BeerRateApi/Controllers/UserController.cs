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
    /// <summary>
    /// Controller for managing user-related operations such as registration, login, token management, and password reminders.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">Service for handling user-related operations.</param>
        public UserController(IUserService userService) { _userService = userService; }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">The user registration details.</param>
        /// <returns>An HTTP response indicating the result of the registration process.</returns>
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

        /// <summary>
        /// Logs in a user and issues JWT and refresh tokens as cookies.
        /// </summary>
        /// <param name="loginDTO">The user login details.</param>
        /// <returns>An HTTP response with user details if successful.</returns>
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

                return Ok(new { loginResult.Id, loginResult.Email, loginResult.Username, loginResult.IsUserAdmin});
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

        /// <summary>
        /// Refreshes the JWT token using the provided refresh token.
        /// </summary>
        /// <returns>A new JWT token and user details.</returns>
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

                return Ok(new { refreshResult.Id, refreshResult.Username, refreshResult.Email, refreshResult.IsUserAdmin });
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

        /// <summary>
        /// Revokes the refresh and JWT tokens of the currently authenticated user.
        /// </summary>
        /// <returns>An HTTP response indicating the revocation result.</returns>
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

        /// <summary>
        /// Sends a password reminder email to the specified address.
        /// </summary>
        /// <param name="remindPasswordDTO">The password reminder details.</param>
        /// <returns>An HTTP response indicating the result.</returns>
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
                return Ok(new { });
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

        /// <summary>
        /// Realizes the password reset process using a provided token and new password.
        /// </summary>
        /// <param name="dto">The password reset details.</param>
        /// <returns>An HTTP response indicating the result.</returns>
        [AllowAnonymous]
        [HttpPost("realise-password-reminding")]
        public async Task<IActionResult> RemindPasswordRealization(ResetPasswordDTO dto)
        {
            try
            {
                await _userService.RealisePasswordReminding(dto.NewPassword, dto.Token);
                return Ok(new { });
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
