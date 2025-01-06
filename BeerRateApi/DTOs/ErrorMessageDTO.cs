namespace BeerRateApi.DTOs
{
    /// <summary>
    /// Represents error messages returned to the client, typically containing a single property for the error description.
    /// </summary>
    public class ErrorMessageDTO
    {
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
