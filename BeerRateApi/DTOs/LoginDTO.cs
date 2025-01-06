using System.ComponentModel.DataAnnotations;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Contains the data required for user login, such as email and password.
    /// </summary>
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public required string Email {  get; set; }
        public required string Password { get; set; }

    }
}
