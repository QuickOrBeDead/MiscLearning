namespace SmsWorker.Model;

public sealed class SmsSentEventLog
{
    public Guid EventId { get; set; }

    public DateTime CreateDate { get; set; }

    public string DocumentId { get; set; }

    public SmsSentEventLog()
    {
        CreateDate = DateTime.Now;
    }
}