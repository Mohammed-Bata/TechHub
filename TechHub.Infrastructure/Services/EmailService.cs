﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TechHub.Application.Interfaces;

namespace TechHub.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmail(string recipient, string subject, string body)
        {
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse("mauricio.torp@ethereal.email"),
                Subject = subject,
            };

            email.To.Add(MailboxAddress.Parse(recipient));
            email.From.Add(new MailboxAddress("Mauricio Torp", "mauricio.torp@ethereal.email"));

            var emailBody = new BodyBuilder();
            emailBody.TextBody = body;
            email.Body = emailBody.ToMessageBody();

            using var Smtp = new SmtpClient();
            await Smtp.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            // Note: Ethereal is a fake SMTP service for testing purposes.
            await Smtp.AuthenticateAsync("mauricio.torp@ethereal.email", "ramYPbaubDqRP9Nk7r");
            await Smtp.SendAsync(email);
            await Smtp.DisconnectAsync(true);
        }
    }
}
