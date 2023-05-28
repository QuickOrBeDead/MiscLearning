namespace EventLogWorker;

using EventLogWorker.Model;

using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Worker : BackgroundService
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
            _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.EventLog", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (_, e) =>
                {
                    try
                    {
                        var eventLog = JsonSerializer.Deserialize<EventLog>(Encoding.UTF8.GetString(e.Body.Span));
                        
                        if (eventLog != null)
                        {
                        }

                        _consumerChannel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        _consumerChannel.BasicNack(e.DeliveryTag, false, true);
                    }
                };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerTag = _consumerChannel.BasicConsume("ElasticSearchEventAnalytics.EventLog", false, consumer);
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
