using System.Security.Claims;

namespace BeerRateApi.Interfaces
{
    /// <summary>
    /// A service for managing token generation and validation.
    /// Includes methods for generating JWT tokens, refresh tokens, and handling expired tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a secure random refresh token.
        /// </summary>
        /// <returns>A base64-encoded refresh token.</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Generates a JSON Web Token (JWT) for authentication.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A JWT as a string.</returns>
        string GenerateJwtToken(string username, int userId);

        /// <summary>
        /// Generates a random string token of 32 characters.
        /// </summary>
        /// <returns>A random 32-character token.</returns>
        string GenerateRandom32Token();

        /// <summary>
        /// Extracts the claims principal from an expired JWT token.
        /// </summary>
        /// <param name="token">The expired JWT token.</param>
        /// <returns>The claims principal if extraction is successful, otherwise <c>null</c>.</returns>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
