﻿using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Enums;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;


namespace BeerRateApi.Services
{
    /// <summary>
    /// A service for managing Users.
    /// Provides methods for user registration, login, password management, token handling, and email operations.
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tokenService">The token service for generating and validating tokens.</param>
        /// <param name="emailService">The email service for sending emails.</param>
        /// <param name="logger">The logger for logging errors and information.</param>
        /// <param name="mapper">The object mapper for DTOs and models.</param>
        /// <param name="configuration">The application configuration settings.</param>
        public UserService(AppDbContext dbContext, ITokenService tokenService, IEmailService emailService, ILogger logger, IMapper mapper, IConfiguration configuration)
            : base(dbContext, logger, mapper)
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _emailService = emailService;
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">The user registration details.</param>
        /// <returns>A <see cref="RegisterResult"/> containing the registered username.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the username or email already exists.</exception>
        /// <exception cref="ArgumentException">Thrown if the password is too short or the username contains forbidden characters.</exception>
        public async Task<RegisterResult> RegisterUser (RegisterDTO registerDTO)
        {
            try
            {
                if (await DbContext.Users.AnyAsync(user => user.Username == registerDTO.Username))
                {
                    throw new InvalidOperationException($"User with username '{registerDTO.Username}' already exists.");
                }

                if (await DbContext.Users.AnyAsync(user => user.Email == registerDTO.Email))
                {
                    throw new InvalidOperationException($"User with email '{registerDTO.Email}' already exists.");
                }

                if (registerDTO.Password.Length < 8)
                {
                    throw new ArgumentException("Password is too short");
                }

                if (!registerDTO.Password.Any(char.IsUpper))
                {
                    throw new ArgumentException("Password needs at least one character [A-Z]");
                }

                if (!registerDTO.Password.Any(char.IsLower))
                {
                    throw new ArgumentException("Password needs at least one character [a-z]");
                }

                if (!registerDTO.Password.Any(char.IsDigit))
                {
                    throw new ArgumentException("Password needs at least one digit");
                }

                if (!registerDTO.Password.Any(c => "!@#$%^&*()".Contains(c)))
                {
                    throw new ArgumentException("Password needs at least one special character: !@#$%^&*()");
                }

                Regex regex = new Regex(@"^[a-zA-Z0-9_]+$");
                if (!regex.IsMatch(registerDTO.Username))
                {
                    throw new ArgumentException("Username contains forbidden characters.");
                }

                var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerDTO.Password);
                var user = new User { Email = registerDTO.Email, Username = registerDTO.Username, PasswordHash=passwordHash };
                DbContext.Users.Add(user);
                await DbContext.SaveChangesAsync();
                return new RegisterResult{Username = registerDTO.Username};
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDTO">The user login credentials.</param>
        /// <returns>A <see cref="LoginResult"/> containing user details and tokens.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the email or password is invalid.</exception>
        public async Task<LoginResult> LoginUser(LoginDTO loginDTO)
        {
            try
            {
                var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email==loginDTO.Email);

                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                if (!BCrypt.Net.BCrypt.EnhancedVerify(loginDTO.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                var refreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = refreshTokenExpiry;
                await DbContext.SaveChangesAsync();

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = refreshTokenExpiry
                };

                var jwtToken = _tokenService.GenerateJwtToken(user.Username, user.Id);

                return new LoginResult { Id=user.Id, 
                    Email=user.Email, 
                    Username=user.Username, 
                    JwtToken=jwtToken, 
                    RefreshTokenExpiry=user.RefreshTokenExpiry, 
                    RefreshToken=user.RefreshToken,  
                    IsUserAdmin=user.UserType==UserType.Admin};
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Refreshes a user's JWT token.
        /// </summary>
        /// <param name="expiredToken">The expired JWT token.</param>
        /// <param name="refreshToken">The associated refresh token.</param>
        /// <returns>A <see cref="LoginResult"/> with a refreshed JWT token.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown for invalid or expired tokens.</exception>
        public async Task<LoginResult> Refresh(string expiredToken, string refreshToken) 
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(expiredToken);
                if (principal?.Identity?.Name == null)
                {
                    throw new UnauthorizedAccessException("Invalid token: could not extract user information.");
                }

                var user = await DbContext.Users.FirstOrDefaultAsync(user => user.Username == principal.Identity.Name);
                if (user == null || refreshToken == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Invalid refresh token or the token has expired.");
                }

                var email = user.Email;
                bool isUserAdmin = user.UserType==UserType.Admin;
                if (email == null)
                {
                    throw new ArgumentNullException(nameof(user.Email), "User email is missing.");
                }

                var idClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (idClaim == null) 
                {
                    throw new InvalidOperationException("Token does not contain a valid user identifier.");
                }

                if (!int.TryParse(idClaim.Value, out var id)) 
                {
                    throw new InvalidOperationException("Invalid user identifier in token.");
                }

                var jwtToken = _tokenService.GenerateJwtToken(principal.Identity.Name,id);

                return new LoginResult { Id = id, Username = principal.Identity.Name, JwtToken=jwtToken, Email=email, IsUserAdmin=isUserAdmin };
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Revokes a user's refresh token.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not found.</exception>
        public async Task Revoke(int id)
        {
            try
            {
                var user = await DbContext.Users.FirstOrDefaultAsync(user => user.Id == id);

                if (user == null) 
                {
                    throw new UnauthorizedAccessException("User not found");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends a password reminder email to the user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not found.</exception>
        public async Task RemindPasswordSendEmail(string email)
        {
            string token = _tokenService.GenerateRandom32Token();
            var clientAddress = _configuration.GetSection("ClientAppSettings")["Address"];
            DateTime expireDate = DateTime.UtcNow.AddHours(6);
            StringBuilder message = new StringBuilder();

            message.AppendLine("Aby przypomnieć hasło kliknij link poniżej: <br/>");
            message.AppendLine($"<a href=\"{clientAddress}/realise-password-reminding/{token}\">{clientAddress}/realise-password-reminding/{token}</a>");

            var user = await DbContext.Users.FirstOrDefaultAsync(user => user.Email == email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            else
            {
                user.RemindPasswordToken = token;
                user.RemindPasswordTokenExpiry = expireDate;
                DbContext.SaveChangesAsync();
                await _emailService.SendAsync(user.Email, "Beer-rate przypomnienie hasła", message.ToString());
            }
        }

        /// <summary>
        /// Resets a user's password using a token.
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <param name="token">The reset token.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown for invalid or expired tokens.</exception>
        /// <exception cref="ArgumentException">Thrown if the password is too short.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the password or token is null.</exception>
        public async Task RealisePasswordReminding(string newPassword, string token)
        {
            User user = await DbContext.Users.FirstOrDefaultAsync(user=>user.RemindPasswordToken == token);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            else if (user.RemindPasswordTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Token expired");
            }
            else if (newPassword.Length < 8)
            {
                throw new ArgumentException("Password is too short");
            }
            else if (newPassword == null)
            {
                throw new ArgumentNullException("Password is null");
            }
            else if (token == null)
            {
                throw new ArgumentNullException("Token is null");
            }
            else
            {
                var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
                user.PasswordHash = passwordHash;
                user.RemindPasswordToken = null;
                user.RemindPasswordTokenExpiry = null;
                DbContext.SaveChanges();
            }    
        }
    }
}
