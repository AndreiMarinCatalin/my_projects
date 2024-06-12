using System;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using RestSharp.Authenticators;
using System.Linq;
using System.Threading;

namespace keymanager_Dev.Clases
{
    public class EmailSender
    {
        public static string Code { get; set; }
        private string destinatary;

        public static async Task SendEmailAsync(string code, string email)
        {
            string fromMail = "keymaganerdev@gmail.com";
            string fromPassword = "sbfzjxlklkuzspjd";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "KEY MANAGER";
            message.To.Add(new MailAddress(email));
            message.Body = $"<html><body> Your verification code is: <strong>{code}</strong> </body><html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }

        public static string GenerateCode(int length)
        {
            Code = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());

            Console.WriteLine("Código generado: " + Code);

            // Reinicia el temporizador cada vez que se genera un nuevo código
            new Timer(InvalidateCode, null, 300000, Timeout.Infinite);

            //300000
            return Code;
        }

        private static void InvalidateCode(object state)
        {
            // Invalida el código
            Code = null;
            Console.WriteLine("El código ha expirado y ha sido invalidado.");
        }
    }
}
