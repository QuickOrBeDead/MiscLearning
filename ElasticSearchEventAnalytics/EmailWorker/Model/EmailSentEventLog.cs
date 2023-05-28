namespace EmailWorker.Model;

public sealed class EmailSentEventLog
{
    public Guid EventId { get; set; }

    public DateTime CreateDate { get; set; }

    public string DocumentId { get; set; }

    public EmailSentEventLog()
    {
        CreateDate = DateTime.Now;
    }
}