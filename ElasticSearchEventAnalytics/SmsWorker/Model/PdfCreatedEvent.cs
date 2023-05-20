namespace SmsWorker.Model;

public sealed class PdfCreatedEvent
{
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; }

    public PdfCreatedEvent()
    {
        Id = Guid.NewGuid();
        CreateDate = DateTime.Now;
    }
}