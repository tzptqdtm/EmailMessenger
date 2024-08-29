using EmailMessenger.Models.Core;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EmailMessenger.Core.Services.Job;

public static class JobScheduleConfigurator
{
    public static void ConfigureNewMessageHandler(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices.GetService<IOptions<JobSchedulerSettings>>()?.Value;
        if (settings == null)
        {
            throw new InvalidOperationException("JobSchedulerSettings not configured properly.");
        }

        RecurringJob.AddOrUpdate<IJobScheduler>("JobScheduler", x => x.ScheduleNewJob(), settings.CronExpression);

        //Для упрощения будем проверять на наличие новых сообщений каждую минуту
        RecurringJob.AddOrUpdate<IJobRunnerService>("MessageSendJobScheduler", x => x.RunNewJob(), "* * * * *");
    }
}