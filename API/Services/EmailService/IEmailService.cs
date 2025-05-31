namespace API.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receptor, string subject, string body);
    }
}