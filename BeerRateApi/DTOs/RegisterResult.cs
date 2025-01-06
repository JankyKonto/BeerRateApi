using BeerRateApi.Models;

namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Encapsulates the result of a user registration process, providing registered user's username.
    /// </summary>
    public class RegisterResult
    {
        public string Username { get; init; } = string.Empty;
    }
}
