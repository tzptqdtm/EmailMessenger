using EmailMessenger.Models.Core;
using EmailMessenger.Models.Data;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailMessenger.Core.Services.Email;

public class MailingService : IMailingService
{
    private readonly ILogger _logger;
    private readonly MailSettings _settings;
    public MailingService(ILogger<MailingService> logger, IOptions<MailSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendMail(MessageEvent messageEvent)
    {
        await SendMail(messageEvent.Message.Body, messageEvent.Message.Subject, messageEvent.Recipient.Address);
    }


    public async Task SendMail(string body, string subject, string recipient)
    {
        //Для упрощения создаем клиент на каждое письмо. В реальном проекте лучше использовать один клиент на все письма
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SmtpSender, _settings.SmtpUser));
            message.To.Add(new MailboxAddress(recipient, recipient));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"<b>{body}</b>" };
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, true);
            await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Not work");
            throw;
        }
    }

}