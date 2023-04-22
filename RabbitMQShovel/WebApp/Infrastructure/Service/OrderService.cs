namespace WebApp.Infrastructure.Service;

using MessageQueue;

using WebApp.Infrastructure.Events;

public interface IOrderService
{
    int CreateOrder();
}

public sealed class OrderService : IOrderService
{
    private readonly IMessageQueuePublisherService _messageQueuePublisherService;

    public OrderService(IMessageQueuePublisherService messageQueuePublisherService)
    {
        _messageQueuePublisherService = messageQueuePublisherService ?? throw new ArgumentNullException(nameof(messageQueuePublisherService));
    }

    public int CreateOrder()
    {
        var @event = new OrderCreatedEvent(1);
        _messageQueuePublisherService.PublishMessage(@event);

        return @event.OrderId;
    }
}