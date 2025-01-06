using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Represents the data required for password recovery, typically the user's email address.
    /// </summary>
    public class RemindPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
    }
}
