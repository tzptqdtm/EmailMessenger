using System.ComponentModel.DataAnnotations;

namespace EmailMessenger.Models.Data;

public class Recipient
{
    [Key]
    public long Id
    {
        get; set;
    }
    public string Address
    {
        get; set;
    }
    public ICollection<Message> Messages
    {
        get; set;
    } = null!;

    public ICollection<MessageEvent> Events
    {
        get; set;
    } = null!;
}