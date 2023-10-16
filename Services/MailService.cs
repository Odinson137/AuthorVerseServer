using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AuthorVerseServer.Services
{
    public class MailService
    {
        private readonly ILogger<MailService> _logger;
        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        public async Task<string> SendEmail(string jwtToken, string mail)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("AuthorVerse", "sanya.baginsky@gmail.com"));
            message.To.Add(new MailboxAddress("Dear user", mail));
            message.Subject = "Confirm your Email";
            message.Body = new BodyBuilder() { HtmlBody = $"<a href = \"http://localhost:3000/AuthorVerse/EmailConfirm?token={jwtToken}\">Для подтверждения почту </a>" }.ToMessageBody();

            try
            {
                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync("sanya.baginsky@gmail.com", "vdsi ujwl keda uxsc");
                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                return "Inputed mail do not exist";
            }
            catch
            {
                return "We have some problem";
            }
            return "Ok";
        }
    }
}
