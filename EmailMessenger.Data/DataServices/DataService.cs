using EmailMessenger.Data.DataContexts;
using EmailMessenger.Models.Data;
using EmailMessenger.Models.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recipient = EmailMessenger.Models.Data.Recipient;

namespace EmailMessenger.Data.DataServices;

public class DataService : IDataService, IDisposable
{
    private readonly ILogger<DataService> _logger;

    private readonly IDataSource _dataSource;
    public DataService(ILogger<DataService> logger, IDataSource dataSource)
    {
        _logger = logger;
        _dataSource = dataSource;
    }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_dataSource != null)
            {
                _dataSource.Dispose();
            }
        }
    }
    #endregion

    public async Task<Message?> GetMessage(Guid id)
    { 
        var message = await _dataSource.Messages
            .Include(m => m.Recipients)
            .Include(m => m.Events)
            .FirstOrDefaultAsync(m => m.Id == id);

        return message;
    }

    public async Task<SendingResult?> GetResult(Guid id)
    {
        var message = await _dataSource.Messages
            .Include(m => m.Recipients)
            .Include(m => m.Events)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            _logger.LogWarning("Message with ID {MessageId} not found.", id);
            return null;
        }

        var sendingResult = new SendingResult
        {
            Id = message.Id,
            Subject = message.Subject,
            Body = message.Body,
            Time = message.Time,
            IsPlanned = message.IsPlanned,
            HasErrors = message.HasErrors,
            Recipients = message.Recipients.Select(r => new EmailMessenger.Models.Web.Recipient
            {
                Id = r.Id,
                Address = r.Address,
                Events = message.Events.Where(n=>n.RecipientId == r.Id)
                    .Select(e => new EmailMessenger.Models.Web.Event
                {
                    Id = e.Id,
                    Result = e.Result,
                    FailedMessage = e.FailedMessage!,
                    Time = e.Time
                }).ToList()
            }).ToList()
        };

        return sendingResult;
    }


    public async Task<Guid> CreateMessageAsync(SendingRequest request)
    {
        var message = new Message
        {
            Subject = request.Subject,
            Body = request.Body,
            Time = DateTime.UtcNow,
            IsPlanned = false,
            IsSent = false,
            HasErrors = false,
            Recipients = request.Recipients
                .Select(r => _dataSource.Recipients.FirstOrDefault(rec => rec.Address == r) ?? new Recipient { Address = r })
                .ToList()
        };

        await _dataSource.Messages.AddAsync(message);

        await _dataSource.SaveChangesAsync();

        return message.Id;
    }

    public async Task<int> UpdateMessageAsync(Message message)
    {
        _dataSource.Entry(message).State = EntityState.Modified;

        return await _dataSource.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetNewMessagesAsync()
    {
        var messages = await _dataSource.Messages
            .Include(m => m.Recipients)
            .Include(m => m.Events)
            .Where(m => !m.IsPlanned)
            .AsNoTracking()
            .ToListAsync();

        return messages;
    }

    public async Task<int> CreateMessageEventAsync(MessageEvent messageEvent)
    {
        await _dataSource.MessageEvents.AddAsync(messageEvent);

        return await _dataSource.SaveChangesAsync();
    }

    public async Task<long?> GetNextMessageEventId()
    {
        var id = await _dataSource.MessageEvents
            .Where(e=>e.Result=="Planned")
            .OrderBy(e => e.Id)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();
        return id;
    }

    public async Task<MessageEvent?> GetMessageEventAsync(long id)
    {
        var messageEvent = await _dataSource.MessageEvents
            .Include(e => e.Message)
            .Include(e => e.Recipient)
            .FirstOrDefaultAsync(e => e.Id == id);

        return messageEvent;
    }

    public async Task<int> SetMessageEventOkAsync(long id)
    {
        var messageEvent = await _dataSource.MessageEvents.FirstOrDefaultAsync(e => e.Id == id);

        if (messageEvent == null)
        {
            return 0;
        }

        messageEvent.Result = "Ok";

        return await _dataSource.SaveChangesAsync();
    }

    public async Task<int> SetMessageEventFailedAsync(long id, string failedMessage)
    {
        var messageEvent = await _dataSource.MessageEvents.FirstOrDefaultAsync(e => e.Id == id);

        if (messageEvent == null)
        {
            return 0;
        }

        messageEvent.Result = "Failed";
        messageEvent.FailedMessage = failedMessage;
        messageEvent.Message.HasErrors = true;
        messageEvent.Time = DateTime.UtcNow;


        return await _dataSource.SaveChangesAsync();
    }

    public async Task<int> SetMessageEventInQueueAsync(long id)
    {
        var messageEvent = await _dataSource.MessageEvents.FirstOrDefaultAsync(e => e.Id == id);

        if (messageEvent == null)
        {
            return 0;
        }

        messageEvent.Result = "InQueue";
        messageEvent.Time = DateTime.UtcNow;

        return await _dataSource.SaveChangesAsync();
    }

}