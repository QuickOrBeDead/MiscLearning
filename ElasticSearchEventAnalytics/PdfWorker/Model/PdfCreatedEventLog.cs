using Nest;

namespace PdfWorker.Model;

public sealed class PdfCreatedEventLog
{
    public Guid EventId { get; set; }

    [Date(Name = "@timestamp")]
    public DateTime CreateDate { get; set; }

    public string DocumentId { get; set; }

    public PdfCreatedEventLog()
    {
        CreateDate = DateTime.Now;
    }
}