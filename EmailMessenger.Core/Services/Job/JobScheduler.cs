using EmailMessenger.Core.Services.Email;
using EmailMessenger.Data.DataServices;
using EmailMessenger.Models.Data;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmailMessenger.Core.Services.Job;

public class JobScheduler : IJobScheduler
{
    private readonly IDataService _dataService;
    private readonly ILogger<JobScheduler> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public JobScheduler(IDataService dataService, ILogger<JobScheduler> logger, IBackgroundJobClient backgroundJobClient)
    {
        _dataService = dataService;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task ScheduleNewJob(Guid guid)
    {
        var message = await _dataService.GetMessage(guid);

        if (message == null)
        {
            return;
        }

        // Добавляем задачи на отправку сообщений каждому получателю в базе данных

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

        _backgroundJobClient.Enqueue<IJobRunnerService>(runner => runner.RunNewJob(guid));

    }
}