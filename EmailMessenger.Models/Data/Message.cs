using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailMessenger.Models.Data;

public class Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id
    {
        get; set;
    }

    public string Subject
    {
        get; set;
    } = null!;

    public string Body
    {
        get; set;
    } = null!;

    public DateTime Time
    {
        get; set;
    }

    public bool IsPlanned
    {
        get; set;
    }

    public bool IsSent
    {
        get; set;
    }

    public bool HasErrors
    {
        get; set;
    }

    public virtual ICollection<Recipient> Recipients
    {
        get; set;
    } = null!;

    public virtual ICollection<MessageEvent> Events
    {
        get; set;
    } = null!;
}