using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Education.Models
{
    public class EmailServices
    {
         public string Msg { get; set; }
         public int Otp { get; set; }

        public async Task<EmailServices> Email(string userEmail)
        {
            // Generate OTP
            Random random = new Random();
            int otp = random.Next(1000, 9999);

            // Send email
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    "ay5332144@gmail.com",
                    "xkuy kedi wprp zriy"   // Gmail App Password
                ),
                EnableSsl = true
            };

            var mail = new MailMessage("ay5332144@gmail.com", userEmail);
            mail.Subject = "Identity Verification Code";
            mail.Body = "Your OTP is: " + otp;

            await client.SendMailAsync(mail);

            string res = "OTP sent on your Email - " + userEmail;

            return new EmailServices()
            {
                Msg = "OTP sent on your Email - " + userEmail,
                Otp = otp
            };
        }
    }
}
