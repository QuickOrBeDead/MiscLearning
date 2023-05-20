using Microsoft.Extensions.Hosting;

using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using PdfWorker.Model;

namespace PdfWorker;

public sealed class Worker : BackgroundService
{
    private readonly IModel _consumerChannel;
    private readonly IConnection _rabbitMqConnection;
    private string? _consumerTag;

    public Worker(IModel consumerChannel, IConnection rabbitMqConnection)
    {
        _consumerChannel = consumerChannel;
        _rabbitMqConnection = rabbitMqConnection;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            _consumerChannel.ExchangeDeclare("ElasticSearchEventAnalytics.OrderCreated", "fanout", false, false, null);
            _consumerChannel.QueueDeclare(queue: "ElasticSearchEventAnalytics.Pdf", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _consumerChannel.QueueBind("ElasticSearchEventAnalytics.Pdf", "ElasticSearchEventAnalytics.OrderCreated", string.Empty);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += (_, e) =>
                {
                    try
                    {
                        var orderCreateEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(Encoding.UTF8.GetString(e.Body.Span));
                        
                        PublishPdfCreatedEvent(orderCreateEvent.Id);

                        _consumerChannel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                        _consumerChannel.BasicNack(e.DeliveryTag, false, true);
                    }
                };
            _consumerChannel.BasicQos(0, 1, false);
            _consumerTag = _consumerChannel.BasicConsume("ElasticSearchEventAnalytics.Pdf", false, consumer);
        }

        return Task.CompletedTask;
    }

    private void PublishPdfCreatedEvent(Guid id)
    {
        using var channel = _rabbitMqConnection.CreateModel();
        channel.ExchangeDeclare("ElasticSearchEventAnalytics.PdfCreated", "fanout", false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new PdfCreatedEvent { Id = id }));

        channel.BasicPublish(exchange: "ElasticSearchEventAnalytics.OrderCreated", routingKey: string.Empty, basicProperties: null, body: body);

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