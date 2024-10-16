using System.ComponentModel.DataAnnotations;

namespace BeerRateApi.DTOs
{
    public class RegisterDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }

        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public required string Email { get; set; }

    }
}
