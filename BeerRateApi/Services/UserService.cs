using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace BeerRateApi.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public UserService(AppDbContext dbContext, ITokenService tokenService, IEmailService emailService, ILogger logger)
            : base(dbContext, logger)
        {
            _tokenService = tokenService;
            _emailService = emailService;
        }

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

                return new LoginResult { Id=user.Id, Email=user.Email, Username=user.Username, JwtToken=jwtToken, RefreshTokenExpiry=user.RefreshTokenExpiry, RefreshToken=user.RefreshToken };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public ResetResult ResetPassword(ResetDTO resetDTO)
        {
            throw new NotImplementedException();
        }

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

                return new LoginResult { Id = id, Username = principal.Identity.Name, JwtToken=jwtToken, Email=email };
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task Revoke(int id)
        {
            try
            {
                var user = await DbContext.Users.FirstOrDefaultAsync(user => user.Id == id);

                if (user == null) 
                {
                    throw new ArgumentNullException("User not found");
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

        public async Task RemindPassword(int id)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user == null)
            {
                throw new ArgumentNullException("User not found");
            }

            await _emailService.SendAsync(user.Email, "Remind password", "This is a test email message with paswword remind");
        }
    }
}
