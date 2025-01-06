using System.ComponentModel.DataAnnotations;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Contains user registration details, such as email, username, and password.
    /// </summary>
    public class RegisterDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }

        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public required string Email { get; set; }

    }
}
