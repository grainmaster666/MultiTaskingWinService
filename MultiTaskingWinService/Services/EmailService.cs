using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using MultiTaskingWinService.Helper;

namespace MultiTaskingWinService.Services
{
    class EmailService : IHostedService, IDisposable
    {
        private Timer _timer;

        readonly MailSettings _mailSettings = null;
        public EmailService(IOptions<MailSettings> options)
        {
            _mailSettings = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => SendEmail(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromDays((int)ServiceInterval.OneHour));

            return Task.CompletedTask;
        }
        public async Task SendEmail()
        {
            MailRequest mailRequest = new MailRequest
            {
                To = "jkpal0@hotmail.com",
                Body = "This is a test message",
                Name = "Jogendra",
                Subject = "Testing"
            };
            await SendEmailAsync( mailRequest);
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                MimeMessage email = new MimeMessage();

                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                email.From.Add(emailFrom);

                
                MailboxAddress emailTo = new MailboxAddress(mailRequest.Name, mailRequest.To);
                email.To.Add(emailTo);

                email.Subject = mailRequest.Subject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = mailRequest.Body;
                email.Body = emailBodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.StackTrace);
                //Log Exception Details
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
