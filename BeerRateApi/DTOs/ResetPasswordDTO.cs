namespace BeerRateApi.DTOs
{
    public class ResetPasswordDTO
    {
        public string NewPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
