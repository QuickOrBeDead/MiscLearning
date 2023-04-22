namespace WebApp.Infrastructure.Service;

using MessageQueue;

using WebApp.Infrastructure.Events;

public interface IOrderService
{
    int CreateOrder();
}

public sealed class OrderService : IOrderService
{
    private static int _orderId = 0;
    private readonly IMessageQueuePublisherService _messageQueuePublisherService;

    public OrderService(IMessageQueuePublisherService messageQueuePublisherService)
    {
        _messageQueuePublisherService = messageQueuePublisherService ?? throw new ArgumentNullException(nameof(messageQueuePublisherService));
    }

    public int CreateOrder()
    {
        var @event = new OrderCreatedEvent(GetNewOrderId());
        _messageQueuePublisherService.PublishMessage(@event);

        return @event.OrderId;
    }

    private static int GetNewOrderId()
    {
        return ++_orderId;
    }
}