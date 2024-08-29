using EmailMessenger.Data.DataServices;
using EmailMessenger.Models.Data;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmailMessenger.Core.Services.Job;

public class JobScheduler : IJobScheduler
{
    private readonly IDataService _dataService;
    private readonly ILogger<JobScheduler> _logger;

    public JobScheduler(IDataService dataService, ILogger<JobScheduler> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
    public async Task ScheduleNewJob()
    {
        var messages = await _dataService.GetNewMessagesAsync();

        // Добавляем задачи на отправку сообщений каждому получателю в базе данных

        foreach (var message in messages)
        {
            foreach (var recipient in message.Recipients)
            {
                var messageEvent = new MessageEvent
                {
                    MessageId = message.Id,
                    Result = "Planned",
                    RecipientId = recipient.Id,
                    Time = DateTime.UtcNow
                };

                await _dataService.CreateMessageEventAsync(messageEvent);
            }

            message.IsPlanned = true;

            await _dataService.UpdateMessageAsync(message);

            _logger.LogInformation("New message: {Id}", message.Id);
        }
    }
}