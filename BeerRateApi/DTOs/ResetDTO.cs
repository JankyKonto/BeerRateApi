namespace BeerRateApi.DTOs
{
    public class ResetDTO
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
