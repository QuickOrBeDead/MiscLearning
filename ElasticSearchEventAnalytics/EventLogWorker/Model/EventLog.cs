using Nest;

namespace EventLogWorker.Model;

public sealed class EventLog
{
    public Guid EventId { get; set; }

    [Keyword()]
    public string EventType { get; set; }

    public string DocumentId { get; set; }

    [Date(Name = "@timestamp")]
    public DateTime CreateDate { get; set; }
}