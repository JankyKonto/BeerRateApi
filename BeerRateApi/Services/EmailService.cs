using BeerRateApi.Interfaces;
using System.Net;
using System.Net.Mail;

namespace BeerRateApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
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
