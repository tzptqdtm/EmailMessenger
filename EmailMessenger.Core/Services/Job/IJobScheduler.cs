namespace EmailMessenger.Core.Services.Job;

public interface IJobScheduler
{
    /// <summary>
    /// Добавляем задачи на отправку сообщений каждому получателю в базе данных
    /// </summary>
    /// <returns></returns>
    Task ScheduleNewJob();
}