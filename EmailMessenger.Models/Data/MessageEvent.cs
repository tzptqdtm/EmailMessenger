using System.ComponentModel.DataAnnotations;

namespace EmailMessenger.Models.Data;

public class MessageEvent
{
    [Key]
    public long Id
    {
        get; set;
    }
    public Guid MessageId
    {
        get; set;
    }

    public virtual Message Message
    {
        get; set;
    } = null!;

    public long RecipientId
    {
        get; set;
    }
    public virtual Recipient Recipient
    {
        get; set;
    } = null!;

    public DateTime Time
    {
        get; set;
    }

    public string Result
    {
        get; set;
    } = null!;

    public string? FailedMessage
    {
        get; set;
    }
}