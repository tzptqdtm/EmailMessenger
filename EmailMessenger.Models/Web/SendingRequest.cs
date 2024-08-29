namespace EmailMessenger.Models.Web;

public class SendingRequest
{
    public string Subject
    {
        get; set;
    } = null!;

    public string Body
    {
        get; set;
    } = null!;

    public string[] Recipients
    {
        get; set;
    } = null!;
}
