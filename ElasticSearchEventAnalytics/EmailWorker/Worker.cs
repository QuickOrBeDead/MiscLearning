using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailWorker;

public sealed class Worker : BackgroundService
{
    private readonly IModel _consumerChannel;
    private string? _consumerTag;

    public Worker(IModel consumerChannel)
    {
        _consumerChannel = consumerChannel;    
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            _consumerChannel.ExchangeDeclare("ElasticSearchEventAnalytics.PdfCreated", "fanout", false, false, null);
            _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.EMail", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _consumerChannel.QueueBind("ElasticSearchEventAnalytics.EMail", "ElasticSearchEventAnalytics.PdfCreated", string.Empty);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (_, e) =>
                {
                    try
                    {
                        _consumerChannel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        _consumerChannel.BasicNack(e.DeliveryTag, false, true);
                    }
                };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerTag = _consumerChannel.BasicConsume("ElasticSearchEventAnalytics.EMail", false, consumer);
        }

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_consumerTag != null)
        {
            _consumerChannel?.BasicCancel(_consumerTag);
        }

        _consumerChannel?.Close(200, "Goodbye");
        _consumerChannel?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}