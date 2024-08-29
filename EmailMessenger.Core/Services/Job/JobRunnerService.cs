using EmailMessenger.Core.Services.Email;
using EmailMessenger.Data.DataServices;
using EmailMessenger.Models.Data;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EmailMessenger.Core.Services.Job;

public class JobRunnerService : IJobRunnerService
{
    private ILogger<JobRunnerService> _logger;
    private readonly IDataService _dataService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    public JobRunnerService(ILogger<JobRunnerService> logger, IDataService dataService, IBackgroundJobClient backgroundJobClient)
    {
        _logger = logger;
        _dataService = dataService;
        _backgroundJobClient = backgroundJobClient;
    }


    public async Task RunNewJob(Guid guid)
    {

        var messageEventsIds = await _dataService.GetMessageEventsIds(guid);

        if (messageEventsIds.Count == 0)
        {
            return;
        }

        foreach (var id in messageEventsIds)
        {
            _backgroundJobClient.Enqueue<IMessageEventHandler>(handler => handler.HandleMessageEvent(id));

            await _dataService.SetMessageEventInQueueAsync(id);
        }

    }
}