using BeerRateApi.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BeerRateApi.Services
{
    /// <summary>
    /// A service for managing token generation and validation.
    /// Includes methods for generating JWT tokens, refresh tokens, and handling expired tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a secure random refresh token.
        /// </summary>
        /// <returns>A base64-encoded refresh token.</returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Generates a JSON Web Token (JWT) for authentication.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A JWT as a string.</returns>
        public string GenerateJwtToken(string username, int userId)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("TokenOptions:Key").Value!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                   {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = _configuration.GetSection("TokenOptions:Issuer").Value,
                Audience = _configuration.GetSection("TokenOptions:Audience").Value
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a random string token of 32 characters.
        /// </summary>
        /// <returns>A random 32-character token.</returns>
        public string GenerateRandom32Token()
        {
            const int length = 32;
            const string chars = "abcdefghijklmnoprstuvwxyz0123456789";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }

        /// <summary>
        /// Extracts the claims principal from an expired JWT token.
        /// </summary>
        /// <param name="token">The expired JWT token.</param>
        /// <returns>The claims principal if extraction is successful, otherwise <c>null</c>.</returns>
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("TokenOptions:Key").Value!);

            var validation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration.GetSection("TokenOptions:Issuer").Value,
                ValidAudience = _configuration.GetSection("TokenOptions:Audience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }
    }
}
