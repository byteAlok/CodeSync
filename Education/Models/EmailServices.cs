using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Education.Helpers;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;

namespace Education.Models
{
    public class EmailServices
    {
        private readonly SiteSettings AppConfig;
        private readonly ILogger<EmailServices> _logger;
        public EmailServices(IOptions<SiteSettings> config, ILogger<EmailServices> logger)
        {
            AppConfig = config.Value;
            _logger = logger;
        }

        public string Msg { get; set; }
         public int Otp { get; set; }

        public async Task<EmailServices> Email(string userEmail)
        {
            // Generate OTP
            Random random = new Random();
            int otp = random.Next(1000, 9999);

            // Send email
            var client = new SmtpClient("smtp.hostinger.com", 587)
            {
                Credentials = new NetworkCredential(
                    SiteData.CompanyEmail,
                    AppConfig.EmailAppPassword
                ),
                EnableSsl = true
            };

            var mail = new MailMessage(SiteData.CompanyEmail, userEmail);
            mail.Subject = "Identity Verification Code";
            mail.Body = "Your OTP is: " + otp;

            await client.SendMailAsync(mail);

            string res = "OTP sent on your Email - " + userEmail;

            this.Msg = "OTP sent on your Email - " + userEmail;
            this.Otp = otp;

            return this;
        }
    }
}
