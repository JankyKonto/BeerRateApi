using BeerRateApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BeerRateApi.Models
{
    public class UserInitializer
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserType UserType { get; set; }
    }
}
