namespace PdfWorker.Model;

public sealed class PdfCreatedEvent
{
    public Guid Guid { get; set; }

    public DateTime CreateDate { get; set; }

    public PdfCreatedEvent()
    {
        Guid = Guid.NewGuid();
        CreateDate = DateTime.Now;
    }
}