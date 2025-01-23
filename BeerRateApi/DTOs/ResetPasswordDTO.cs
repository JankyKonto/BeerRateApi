namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Contains data for resetting a user's password, requiring a new password and a verification token.
    /// </summary>
    public class ResetPasswordDTO
    {
        public string NewPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
