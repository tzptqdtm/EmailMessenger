namespace EmailMessenger.Models.Web;

public class Event
{
    public long Id
    {
        get; set;
    }
    public DateTime Time
    {
        get; set;
    }
    public string Result
    {
        get; set;
    }
    public string FailedMessage
    {
        get; set;
    }
}

public class Recipient
{
    public long Id
    {
        get; set;
    }
    public string Address
    {
        get; set;
    }
    public List<Event> Events
    {
        get; set;
    }
}

public class SendingResult
{
    public Guid Id
    {
        get; set;
    }
    public string Subject
    {
        get; set;
    }
    public string Body
    {
        get; set;
    }
    public DateTime Time
    {
        get; set;
    }
    public bool IsPlanned
    {
        get; set;
    }
    public bool HasErrors
    {
        get; set;
    }
    public List<Recipient> Recipients
    {
        get; set;
    }
}