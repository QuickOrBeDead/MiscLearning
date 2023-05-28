namespace EventLogWorker.Model;

public sealed class EventLog
{
    public Guid EventId { get; set; }

    public DateTime CreateDate { get; set; }

    public string DocumentId { get; set; }
}