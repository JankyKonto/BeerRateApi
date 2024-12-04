namespace BeerRateApi.DTOs
{
    public class LoginResult
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsUserAdmin { get; set; } = false;
    }
}
