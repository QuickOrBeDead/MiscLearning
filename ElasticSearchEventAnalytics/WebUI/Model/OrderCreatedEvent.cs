namespace WebUI.Model;

public sealed class OrderCreatedEvent
{
    public Guid Guid { get; set; }

    public DateTime CreateDate { get; set; }

    public OrderCreatedEvent()
    {
        Guid = Guid.NewGuid();
        CreateDate = DateTime.Now;
    }
}