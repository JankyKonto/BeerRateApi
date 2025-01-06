namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Describes a user entity, including properties like ID, username and email.
    /// </summary>
    public class UserDTO
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
