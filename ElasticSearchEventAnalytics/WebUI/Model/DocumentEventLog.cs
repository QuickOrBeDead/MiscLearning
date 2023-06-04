namespace WebUI.Model;

public sealed class DocumentEventLog
{
    public Guid EventId { get; set; }

    public string DocumentId { get; set; }

    public bool? IsPdfCreated { get; set; }

    public bool? IsSmsSent { get; set; }

    public bool? IsEmailSent { get; set; }

    public bool? IsDocumentViewed { get; set; }

    public DateTime CreateDate { get; set; }

    public DocumentEventLog()
    {
        CreateDate = DateTime.Now;
    }
}