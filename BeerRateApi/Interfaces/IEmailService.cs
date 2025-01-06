namespace BeerRateApi.Interfaces
{
    /// <summary>
    /// An interface for managing email sending
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email in HTML format.</param>
        /// <exception cref="ArgumentNullException">Thrown when email sender settings are invalid or missing.</exception>
        Task SendAsync(string toEmail, string subject, string content);

    }
}
