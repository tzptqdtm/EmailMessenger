using EmailMessenger.Models.Data;

namespace EmailMessenger.Core.Services.Email;

public interface IMailingService
{
    /// <summary>
    /// Метод отправляет письмо указанному получателю
    /// </summary>
    /// <param name="body"></param>
    /// <param name="subject"></param>
    /// <param name="recipient"></param>
    /// <returns></returns>
    Task SendMail(string body, string subject, string recipient);
    /// <summary>
    /// Метод отправляет письмо указанному получателю
    /// </summary>
    /// <param name="messageEvent"></param>
    /// <returns></returns>
    Task SendMail(MessageEvent messageEvent);
}