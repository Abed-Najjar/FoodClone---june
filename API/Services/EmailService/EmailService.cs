using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public async Task SendEmailAsync(string receptor, string subject, string body)
        {
            try
            {
                var email = configuration.GetValue<string>("EMAIL_CONFIGURATION:EMAIL");
                var password = configuration.GetValue<string>("EMAIL_CONFIGURATION:PASSWORD");
                var host = configuration.GetValue<string>("EMAIL_CONFIGURATION:HOST");
                var port = configuration.GetValue<int>("EMAIL_CONFIGURATION:PORT");

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(host))
                {
                    throw new InvalidOperationException("Email configuration is incomplete. Please check EMAIL_CONFIGURATION settings.");
                }

                using var smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(email, password);

                using var message = new MailMessage(email, receptor, subject, body);
                message.IsBodyHtml = true; // Enable HTML content for OTP emails
                
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Log the error (you can inject ILogger if needed)
                throw new InvalidOperationException($"Failed to send email to {receptor}: {ex.Message}", ex);
            }
        }


    }
}