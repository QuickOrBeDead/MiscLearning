namespace WebUI.Model;

public sealed class EventLog
{
    public Guid EventId { get; set; }

    public string EventType { get; set; }

    public DateTime CreateDate { get; set; }

    public string DocumentId { get; set; }

    public EventLog()
    {
        CreateDate = DateTime.Now;
    }
}