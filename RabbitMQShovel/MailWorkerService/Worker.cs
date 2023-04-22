namespace MailWorkerService;

using System.Collections.Concurrent;

using MailWorkerService.Infrastructure.Events;

using MessageQueue;
using System.Net.Mail;
using System.Net;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IMessageQueueConsumerService<OrderCreatedEvent> _messageQueueConsumerService;

    private readonly ConcurrentDictionary<int, int> _tryCounts = new();

    public Worker(ILogger<Worker> logger, IMessageQueueConsumerService<OrderCreatedEvent> messageQueueConsumerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageQueueConsumerService = messageQueueConsumerService ?? throw new ArgumentNullException(nameof(messageQueueConsumerService));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            _messageQueueConsumerService.ConsumeMessage(
                (m, _) =>
                    {
                        var tryCount = _tryCounts.AddOrUpdate(m.OrderId, _ => 1, (_, x) => x + 1);
                        if (tryCount >= 3)
                        {
                            return true;
                        }

                        throw new InvalidOperationException("Error test");
                    });
        }

        return Task.CompletedTask;
    }
}