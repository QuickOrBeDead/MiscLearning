using Nest;

namespace EventLogWorker.Model;

 [ElasticsearchType(IdProperty = "EventId")]
public sealed class DocumentEventLog
{
    public Guid EventId { get; set; }

    [Keyword]
    public string DocumentId { get; set; }

    public bool? IsPdfCreated { get; set; }

    public bool? IsSmsSent { get; set; }

    public bool? IsEmailSent { get; set; }

    public bool? IsDocumentViewed { get; set; }

    [Date(Name = "@timestamp")]
    public DateTime CreateDate { get; set; }

    public DocumentEventLog()
    {
        CreateDate = DateTime.Now;
    }
}