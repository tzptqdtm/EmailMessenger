namespace EmailMessenger.Core.Services.Email;

public interface IMessageEventHandler
{
    /// <summary>
    /// Обработка события отправки сообщения с указанным id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task HandleMessageEvent(long id);
}