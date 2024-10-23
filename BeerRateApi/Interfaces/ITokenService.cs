using System.Security.Claims;

namespace BeerRateApi.Interfaces
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        string GenerateJwtToken(string username, int userId);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
