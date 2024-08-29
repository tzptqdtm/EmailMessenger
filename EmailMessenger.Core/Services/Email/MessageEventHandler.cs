using System.Net.Mail;
using EmailMessenger.Data.DataServices;
using Microsoft.Extensions.Logging;

namespace EmailMessenger.Core.Services.Email;

public class MessageEventHandler : IMessageEventHandler
{
    private readonly ILogger<MessageEventHandler> _logger;
    private readonly IMailingService _mailingService;
    private readonly IDataService _dataService;

    public MessageEventHandler(IMailingService mailingService, ILogger<MessageEventHandler> logger, IDataService dataService)
    {
        _logger = logger;
        _mailingService = mailingService;
        _dataService = dataService;
    }

    public async Task HandleMessageEvent(long id)
    {
        var messageEvent = await _dataService.GetMessageEventAsync(id);

        if (messageEvent == null)
        {
            _logger.LogWarning("Message event with id {Id} not found", id);
            return;
        }

        try
        {
            // Для упрощения используем класс MailAddress для проверки валидности адреса. В реальном проекте лучше использовать другую проверку
            var mail = new MailAddress(messageEvent.Recipient.Address);

            await _mailingService.SendMail(messageEvent);
        }
        catch (FormatException ex)
        {
            await _dataService.SetMessageEventFailedAsync(id, "Invalid email address.");
            _logger.LogError(ex,"Invalid email address. Address = {0}", messageEvent.Recipient.Address);
            return;
        }
        catch (Exception e)
        {
            await _dataService.SetMessageEventFailedAsync(id, e.Message);
            _logger.LogError(e, "Error sending email to {0}", messageEvent.Recipient.Address);
            return;
        }

        await _dataService.SetMessageEventOkAsync(id);
    }

}