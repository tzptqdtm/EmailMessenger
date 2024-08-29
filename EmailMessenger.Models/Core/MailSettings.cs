namespace EmailMessenger.Models.Core;

public class MailSettings
{
    public string SmtpServer
    {
        get; set;
    }
    public int SmtpPort
    {
        get; set;
    }
    public string SmtpUser
    {
        get; set;
    }
    public string SmtpPassword
    {
        get; set;
    }
    public string SmtpSender
    {
        get; set;
    }
}
