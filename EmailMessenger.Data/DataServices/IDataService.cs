using EmailMessenger.Models.Data;
using EmailMessenger.Models.Web;

namespace EmailMessenger.Data.DataServices;

public interface IDataService
{
    /// <summary>
    /// Возвращает сообщение по указанному id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Message?> GetMessage(Guid id);
    /// <summary>
    /// Возвращает результат отправки сообщения по указанному id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SendingResult?> GetResult(Guid id);
    /// <summary>
    /// Создает сообщение на отправку в базе данных
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Guid> CreateMessageAsync(SendingRequest request);
    /// <summary>
    /// Получает новые сообщения на отправку из базы данных
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Message>> GetNewMessagesAsync();
    /// <summary>
    /// Обновляет сообщение в базе данных
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<int> UpdateMessageAsync(Message message);
    /// <summary>
    /// Создает событие отправки сообщения конкретному адресату, в базе данных
    /// </summary>
    /// <param name="messageEvent"></param>
    /// <returns></returns>
    Task<int> CreateMessageEventAsync(MessageEvent messageEvent);

    /// <summary>
    /// Получает следующий id события отправки сообщения
    /// </summary>
    /// <returns></returns>
    Task<List<long>> GetMessageEventsIds(Guid guid);
    /// <summary>
    /// Получает событие отправки сообщения по указанному id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<MessageEvent?> GetMessageEventAsync(long id);
    /// <summary>
    /// Устанавливает статус события отправки сообщения "Ошибка" и сохраняет сообщение об ошибке
    /// </summary>
    /// <param name="id"></param>
    /// <param name="failedMessage"></param>
    /// <returns></returns>
    Task<int> SetMessageEventFailedAsync(long id, string failedMessage);
    /// <summary>
    /// Устанавливает статус события отправки сообщения "Успешно"
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<int> SetMessageEventOkAsync(long id);
    /// <summary>
    /// Устанавливает статус события отправки сообщения "В очереди"
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<int> SetMessageEventInQueueAsync(long id);
}