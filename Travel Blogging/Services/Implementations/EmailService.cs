using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Services.Implementations
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpService = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];


            using var client = new System.Net.Mail.SmtpClient(smtpService, port)
            {
                Credentials = new System.Net.NetworkCredential(username, password),
                EnableSsl=true
            };

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }

    }
}
