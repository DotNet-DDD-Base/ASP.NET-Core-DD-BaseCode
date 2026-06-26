namespace UserApp.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string body);
    Task SendTemplateAsync(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders);
}
