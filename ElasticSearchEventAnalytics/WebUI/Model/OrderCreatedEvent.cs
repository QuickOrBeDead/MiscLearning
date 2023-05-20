namespace WebUI.Model;

public sealed class OrderCreatedEvent
{
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; }

    public OrderCreatedEvent()
    {
        Id = Guid.NewGuid();
        CreateDate = DateTime.Now;
    }
}