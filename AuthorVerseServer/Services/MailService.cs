using AuthorVerseServer.Interfaces;
using AuthorVerseServer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;

namespace AuthorVerseServer.Services
{
    public class MailService
    {
        private readonly IUser _user;
        private readonly IBookChapter _bookChapter;
        //public MailService() {}

        public MailService(IUser user, IBookChapter bookChapter)
        {
            _user = user;
            _bookChapter = bookChapter;
        }

        public virtual async Task<string> SendEmail(string jwtToken, string mail)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("AuthorVerse", "sanya.baginsky@gmail.com"));
            message.To.Add(new MailboxAddress("Dear user", mail));
            message.Subject = "Confirm your Email";
            message.Body = new BodyBuilder() { HtmlBody = $"<a href = \"http://localhost:3000/AuthorVerse/EmailConfirm?token={jwtToken}\">Для подтверждения почты перейдите по ссылке</a>" }.ToMessageBody();

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
                return "Inputted mail do not exist";
            }
            catch
            {
                return "We have some problem";
            }
            return "Ok";
        }

        public virtual async Task<bool> SendNotifyEmail(int chapterId)
        {
            var chapter = await _bookChapter.GetNotifyChapterAsync(chapterId);
            var emails = await _user.GetUserEmailAsync(chapter.BookId);
            if (emails.Count() == 0)
            {
                Console.WriteLine("Пользователей для отправки сообщений не найдено");
                return true;
            }

            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("AuthorVerse", "sanya.baginsky@gmail.com"));
            message.Subject = "New chapter";

            string titleChapter = string.IsNullOrEmpty(chapter.ChapterTitle) 
                ? chapter.ChapterNumber.ToString() 
                : $"{chapter.ChapterNumber} - {chapter.ChapterTitle}";

            string imageUrl;
            if (!string.IsNullOrEmpty(chapter.Url))
            {
                byte[] imageData;
                using (HttpClient httpClient = new HttpClient())
                {
                    imageData = await httpClient.GetByteArrayAsync($"http://localhost:7069/api/images/{chapter.Url}");
                }
                string imageDataUri = $"data:image/png;base64,{Convert.ToBase64String(imageData)}";
                imageUrl = $"<img src='{imageDataUri}' alt='Chapter Image'></img>";

            }
            else
                imageUrl = string.Empty;
            

            message.Body = new BodyBuilder()
            {
                HtmlBody = $"<h3>Вышла новая глава </h3>" +
                            $"<h2>{titleChapter} </h2> книги из вашей библиотеки <h2>{chapter.BookTitle}</h2>. {imageUrl}"
            }.ToMessageBody();

            try
            {
                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync("sanya.baginsky@gmail.com", "vdsi ujwl keda uxsc");
                    
                    foreach (var email in emails)
                    {
                        message.To.Clear();
                        message.To.Add(new MailboxAddress("Dear user", email));

                        await client.SendAsync(message);
                    }

                    await client.DisconnectAsync(true);
                }
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                Console.WriteLine($"{ex}\nОшибка при нахождение такого email");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }
    }
}
