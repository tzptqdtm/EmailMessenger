namespace EmailMessenger.Core.Services.Job;

public interface IJobRunnerService
{
    /// <summary>
    /// Добавляет задачу по отправке новых сообщений в очередь на выполнение
    /// </summary>
    /// <returns></returns>
    Task RunNewJob();
}