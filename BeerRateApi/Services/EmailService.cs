using BeerRateApi.Interfaces;
using System.Net;
using System.Net.Mail;

namespace BeerRateApi.Services
{
    /// <summary>
    /// A service for managing email sending
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for EmailService.
        /// </summary>
        /// <param name="configuration">Application configuration to retrieve email sender settings.</param>
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email in HTML format.</param>
        /// <exception cref="ArgumentNullException">Thrown when email sender settings are invalid or missing.</exception>
        public async Task SendAsync(string toEmail, string subject, string body)
        {
            
            var host = _configuration.GetSection("EmailSenderSettings")["Host"];
            var hostAddress = _configuration.GetSection("EmailSenderSettings")["HostAddress"];
            var appPassword = Environment.GetEnvironmentVariable("GOOGLE_APP_PASSWORD");
            
            if(host == null || hostAddress == null || appPassword == null)
            {
                throw new ArgumentNullException("Wrong email sender settings");
            }

            var smptClient = new SmtpClient(host)
            {
                Port = 587,
                Credentials = new NetworkCredential(hostAddress, appPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(hostAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(toEmail));
            await smptClient.SendMailAsync(mailMessage);
        }
    }
}
