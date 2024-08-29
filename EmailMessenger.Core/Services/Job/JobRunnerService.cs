﻿using EmailMessenger.Core.Services.Email;
using EmailMessenger.Data.DataServices;
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
    

    public async Task RunNewJob()
    {
        while (true)
        {
            var messageEventId = await _dataService.GetNextMessageEventId();

            if (messageEventId == null || messageEventId.Value == 0)
            {
                break;
            }

            _backgroundJobClient.Enqueue<IMessageEventHandler>(handler => handler.HandleMessageEvent(messageEventId.Value));

            await _dataService.SetMessageEventInQueueAsync(messageEventId.Value);

        }
    }
    
}