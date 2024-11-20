using System.ComponentModel.DataAnnotations;

namespace BeerRateApi.DTOs
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public required string Email {  get; set; }
        public required string Password { get; set; }

    }
}
