namespace BeerRateApi.Interfaces
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        string GenerateJwtToken(string username);

    }
}
