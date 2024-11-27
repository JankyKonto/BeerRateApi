using BeerRateApi.Enums;

namespace BeerRateApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public UserType UserType { get; set; }
        public string? RemindPasswordToken { get; set; }
        public DateTime? RemindPasswordTokenExpiry { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
