using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace UCTrafficApp.Services
{
    public class EmailService
    {
        // 🔹 Your Mailtrap sandbox credentials
        private const string Host = "sandbox.smtp.mailtrap.io";
        private const int Port = 587; // 587 works best for STARTTLS
        private const string User = "1535cb7c0890fa";   // your Mailtrap username
        private const string Pass = "5a13ea8d9b2a3e";         // your Mailtrap password (full value)
        private const string From = "no-reply@uctraffic.com";

        public async Task SendVerificationCodeAsync(string toEmail, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("UC Traffic", From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "UC Traffic - Email Verification";
            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {code}\n\nPlease enter it in the UC Traffic app to verify your account."
            };

            using var client = new SmtpClient();

            // This bypasses strict certificate validation (safe for development)
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(User, Pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
