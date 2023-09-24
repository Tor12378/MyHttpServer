using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer
{
    internal class Sender
    {
        public static async Task SendEmailAsync()
        {
            MailAddress from = new MailAddress("testhw@rambler.ru", "Tom");
            MailAddress to = new MailAddress("testhw@rambler.ru");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Тест";
            m.Body = "Письмо-тест 2 работы smtp-клиента";
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 465);
            smtp.Credentials = new NetworkCredential("testhw@rambler.ru", "testHwtestHw1");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
            Console.WriteLine("Письмо отправлено");
        }
    }
}
